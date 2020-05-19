using System;
using System.Collections.Generic;
using System.Security.Policy;
using Backend.Controllers.Dto;
using Backend.Managers;

namespace Backend.Models
{
    public class Room
    {
        private static readonly TimeSpan idleTimeout = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan fireTimeout = TimeSpan.FromSeconds(15);
        private static readonly HashSet<RoomStatus> activeRoomStatuses = new HashSet<RoomStatus>{RoomStatus.EmptyRoom, RoomStatus.NotReady, RoomStatus.Ready};
        private readonly PlayerBuilder playerBuilder;

        public Room(PlayerBuilder playerBuilder)
        {
            this.playerBuilder = playerBuilder;
            LastActivityTicks = DateTime.Now.Ticks;
        }

        public bool HasActiveStatus => activeRoomStatuses.Contains(Status);
        public Guid Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public RoomStatus Status { get; set; }
        public long LastActivityTicks { get; set; }
        public Guid CurrentPlayerId { get; set; }

        public CreateRoomResponseDto Enter(CreateRoomRequestDto requestDto)
        {
            UpdateStatusIfNeeded();

            if (Status != RoomStatus.EmptyRoom && Status != RoomStatus.NotReady)
                throw new InvalidOperationException("Room already filled");

            LastActivityTicks = DateTime.Now.Ticks;

            var player = playerBuilder.Build(requestDto.PlayerId, requestDto.PlayerName);

            if (Player1 == null)
            {
                Player1 = player;
                Status = RoomStatus.NotReady;
                return new CreateRoomResponseDto
                {
                    PlayerId = player.Id,
                    RoomId = Id,
                    RoomStatus = RoomStatus.NotReady,
                    AnotherPlayerName = null
                };
            }

            Player2 = player;
            Status = RoomStatus.Ready;
            CurrentPlayerId = Player1.Id;

            return new CreateRoomResponseDto
            {
                PlayerId = player.Id,
                RoomId = Id,
                RoomStatus = RoomStatus.Ready,
                AnotherPlayerName = Player1.Name
            };
        }

        public FireResponseDto Fire(FireRequestDto dto, Guid playerId)
        {
            UpdateStatusIfNeeded();

            if (Status != RoomStatus.Ready)
                throw new InvalidOperationException("Room is not ready");            

            LastActivityTicks = DateTime.Now.Ticks;

            var enemyPlayer = GetEnemyPlayer(playerId);
            var fireResult = enemyPlayer.ProcessEnemyMove(dto.X, dto.Y);
            CurrentPlayerId = fireResult != FireResult.Missed
                ? playerId
                : enemyPlayer.Id;

            Status = !enemyPlayer.AnyShipsAlive() ? RoomStatus.Finished : Status;
            return new FireResponseDto
            {
                EnemyMap = enemyPlayer.OwnMap.ToMapForEnemyDto()
            };
        }

        public GetRoomStatusResponseDto GetStatus(Guid playerId)
        {
            UpdateStatusIfNeeded();
            LastActivityTicks = DateTime.Now.Ticks;

            return new GetRoomStatusResponseDto
            {
                PlayerId = playerId,
                RoomId = Id,
                RoomStatus = Status,
                AnotherPlayerName = GetEnemyPlayer(playerId)?.Name
            };
        }

        public GetGameStatusResponseDto GetGameStatus(Guid playerId)
        {
            UpdateStatusIfNeeded();

            if (Status == RoomStatus.Orphaned)
            {
                return new GetGameStatusResponseDto
                {
                    FinishReason = FinishReason.ConnectionLost,
                    GameStatus = GameStatus.Finish,
                    MyMap = GetMyPlayer(playerId).OwnMap.ToMapDto(),
                    YourChoiceTimeout = TimeSpan.Zero
                };
            }

            var gameStatus = Status == RoomStatus.Finished
                ? GameStatus.Finish
                : CurrentPlayerId == playerId
                    ? GameStatus.YourChoice
                    : GameStatus.PendingForFriendChoice;
            return new GetGameStatusResponseDto
            {
                YourChoiceTimeout = gameStatus == GameStatus.YourChoice ? fireTimeout : TimeSpan.Zero,
                MyMap = GetMyPlayer(playerId).OwnMap.ToMapDto(),
                GameStatus = gameStatus,
                FinishReason = gameStatus == GameStatus.Finish
                    ? CurrentPlayerId == playerId
                        ? FinishReason.Winner
                        : FinishReason.Lost
                    : (FinishReason?) null
            };
        }

        private Player GetEnemyPlayer(Guid playerId) =>
            Player1.Id == playerId ? Player2 : Player1;

        private Player GetMyPlayer(Guid playerId) =>
            Player1.Id == playerId ? Player1 : Player2;

        public void UpdateStatusIfNeeded()
        {
            var currentTicks = DateTime.Now.Ticks;
            if (HasActiveStatus && TimeSpan.FromTicks(currentTicks - LastActivityTicks) > idleTimeout)
            {
                Status = RoomStatus.Orphaned;
            }
        }
    }
}