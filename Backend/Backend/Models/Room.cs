using System;
using Backend.Controllers.Dto;
using Backend.Managers;

namespace Backend.Models
{
    public class Room
    {
        private readonly PlayerBuilder playerBuilder;

        public Room(PlayerBuilder playerBuilder) =>
            this.playerBuilder = playerBuilder;

        public Guid Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public bool GameFinished { get; set; }
        public RoomStatus Status { get; set; }
        public long LastMoveTicks { get; set; }
        public Guid CurrentPlayerId { get; set; }

        public CreateRoomResponseDto Enter(CreateRoomRequestDto requestDto)
        {
            if (Status == RoomStatus.Ready)
                throw new InvalidOperationException("Room already filled");

            var player = playerBuilder.Build(requestDto.PlayerName);

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
            LastMoveTicks = DateTime.Now.Ticks;

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
            LastMoveTicks = DateTime.Now.Ticks;

            var enemyPlayer = GetEnemyPlayer(playerId);
            var fireResult = enemyPlayer.ProcessEnemyMove(dto.X, dto.Y);
            CurrentPlayerId = fireResult != FireResult.Missed
                ? playerId
                : enemyPlayer.Id;

            GameFinished = !enemyPlayer.AnyShipsAlive();
            return new FireResponseDto
            {
                EnemyMap = enemyPlayer.OwnMap.ToMapForEnemyDto()
            };
        }

        public GetRoomStatusResponseDto GetStatus(Guid playerId) =>
            new GetRoomStatusResponseDto
            {
                PlayerId = playerId,
                RoomId = Id,
                RoomStatus = Status,
                AnotherPlayerName = GetEnemyPlayer(playerId)?.Name
            };

        public GetGameStatusResponseDto GetGameStatus(Guid playerId)
        {
            var currentTicks = DateTime.Now.Ticks;
            if (TimeSpan.FromTicks(currentTicks - LastMoveTicks) > TimeSpan.FromSeconds(60))
            {
                GameFinished = true;
                return new GetGameStatusResponseDto
                {
                    FinishReason = FinishReason.ConnectionLost,
                    GameStatus = GameStatus.Finish,
                    MyMap = GetMyPlayer(playerId).OwnMap.ToMapDto(),
                    YourChoiceTimeout = TimeSpan.Zero
                };
            }

            var gameStatus = GameFinished
                ? GameStatus.Finish
                : CurrentPlayerId == playerId
                    ? GameStatus.YourChoice
                    : GameStatus.PendingForFriendChoice;
            return new GetGameStatusResponseDto
            {
                YourChoiceTimeout = gameStatus == GameStatus.YourChoice ? TimeSpan.FromSeconds(15) : TimeSpan.Zero,
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
    }
}