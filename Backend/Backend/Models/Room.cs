using System;
using System.Collections.Generic;
using System.Security.Policy;
using Backend.Controllers.v1.Dto;
using Backend.Managers;

namespace Backend.Models
{
    public class Room
    {
        private static readonly TimeSpan idleTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan fireTimeout = TimeSpan.FromSeconds(15);
        private static readonly HashSet<RoomStatus> openedRoomStatuses = new HashSet<RoomStatus>{RoomStatus.EmptyRoom, RoomStatus.NotReady};
        private static readonly HashSet<RoomStatus> activeRoomStatuses = new HashSet<RoomStatus>{RoomStatus.EmptyRoom, RoomStatus.NotReady, RoomStatus.Ready};
        private readonly PlayerBuilder playerBuilder;
        private RoomStatus status;

        public Room(PlayerBuilder playerBuilder)
        {
            this.playerBuilder = playerBuilder;
            Touch();
        }

        public bool IsActive => activeRoomStatuses.Contains(Status);
        public bool IsOpened => openedRoomStatuses.Contains(Status);
        public Guid Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public RoomStatus Status
        {
            get
            {
                var currentTicks = DateTime.Now.Ticks;
                if (IsActive && TimeSpan.FromTicks(currentTicks - LastActivityTicks) > idleTimeout)
                {
                    status = RoomStatus.Orphaned;
                }

                return status;
            }
            set => status = value;
        }

        public long LastActivityTicks { get; set; }
        public Guid CurrentPlayerId { get; set; }

        public Player Enter(Guid playerId, string playerName)
        {
            if (Status != RoomStatus.EmptyRoom && Status != RoomStatus.NotReady)
                throw new InvalidOperationException("Room already filled");

            Touch();

            var player = playerBuilder.Build(playerId, playerName);

            if (Player1 == null)
            {
                Player1 = player;
                Status = RoomStatus.NotReady;
                return Player1;
            }

            if (Player1.Id == playerId)
            {
                throw new ArgumentException("Can't append player twice", nameof(playerId));
            }

            Player2 = player;
            Status = RoomStatus.Ready;
            CurrentPlayerId = Player1.Id;
            return Player2;
        }

        public FireResponse Fire(int x, int y, Guid playerId)
        {
            if (Status != RoomStatus.Ready)
                throw new InvalidOperationException("Room is not ready");

            Touch();

            var enemyPlayer = GetEnemyPlayerFor(playerId);
            var fireResult = enemyPlayer.ProcessEnemyMove(x, y);
            CurrentPlayerId = fireResult != FireResult.Missed
                ? playerId
                : enemyPlayer.Id;

            Status = !enemyPlayer.AnyShipsAlive() ? RoomStatus.Finished : Status;
            return new FireResponse
            {
                EnemyMap = enemyPlayer.OwnMap
            };
        }

        [Obsolete("Временны костыль")]
        public void Touch() =>
            LastActivityTicks = DateTime.Now.Ticks;

        public Game GetGameFor(Guid playerId)
        {
            if (Status == RoomStatus.Orphaned)
            {
                return new Game
                {
                    FinishReason = FinishReason.ConnectionLost,
                    GameStatus = GameStatus.Finish,
                    MyMap = GetMyPlayer(playerId).OwnMap,
                    YourChoiceTimeout = TimeSpan.Zero
                };
            }

            var gameStatus = Status == RoomStatus.Finished
                ? GameStatus.Finish
                : CurrentPlayerId == playerId
                    ? GameStatus.YourChoice
                    : GameStatus.PendingForFriendChoice;
            return new Game
            {
                YourChoiceTimeout = gameStatus == GameStatus.YourChoice ? fireTimeout : TimeSpan.Zero,
                MyMap = GetMyPlayer(playerId).OwnMap,
                GameStatus = gameStatus,
                FinishReason = gameStatus == GameStatus.Finish
                    ? CurrentPlayerId == playerId
                        ? FinishReason.Winner
                        : FinishReason.Lost
                    : (FinishReason?) null
            };
        }

        public Player GetEnemyPlayerFor(Guid playerId) =>
            Player1.Id == playerId ? Player2 : Player1;

        private Player GetMyPlayer(Guid playerId) =>
            Player1.Id == playerId ? Player1 : Player2;
    }
}