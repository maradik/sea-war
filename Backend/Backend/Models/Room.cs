using System;
using Backend.Controllers;

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
            var enemyPlayer = Player1.Id == playerId ? Player2 : Player1;
            var moveResult = enemyPlayer.ProcessEnemyMove(dto.X, dto.Y) == CellStatus.EngagedByShipFired;
            CurrentPlayerId = moveResult
                ? playerId
                : enemyPlayer.Id;
            var gameFinished = true;
            foreach (var cell in enemyPlayer.OwnMap.Cells)
            {
                if (cell.Status == CellStatus.EngagedByShip)
                {
                    gameFinished = false;
                    break;
                }
            }

            GameFinished = gameFinished;
            return new FireResponseDto
            {
                EnemyMap = enemyPlayer.OwnMap
            };
        }

        public GetRoomStatusResponseDto GetStatus(Guid playerId) =>
            new GetRoomStatusResponseDto
            {
                PlayerId = playerId,
                RoomId = Id,
                RoomStatus = Status,
                AnotherPlayerName = Player1.Id == playerId ? Player2.Name : Player1.Name
            };

        public GetGameStatusResponseDto GetGameStatus(Guid playerId)
        {
            var gameStatus = GameFinished
                ? GameStatus.Finish
                : CurrentPlayerId == playerId
                    ? GameStatus.YourChoice
                    : GameStatus.PendingForFriendChoice;
            return new GetGameStatusResponseDto
            {
                YourChoiceTimeout = gameStatus == GameStatus.YourChoice ? TimeSpan.FromSeconds(30) : TimeSpan.Zero,
                MyMap = Player1.Id == playerId ? Player1.OwnMap : Player2.OwnMap,
                GameStatus = gameStatus,
                FinishReason = gameStatus == GameStatus.Finish
                    ? CurrentPlayerId == playerId
                        ? FinishReason.Winner
                        : FinishReason.Lost
                    : (FinishReason?) null
            };
        }
    }
}