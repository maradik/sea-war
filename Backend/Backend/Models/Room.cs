using System;
using Backend.Controllers;

namespace Backend.Models
{
    public class Room : IRoom
    {
        private readonly PlayerBuilder playerBuilder;

        public Room(PlayerBuilder playerBuilder)
        {
            this.playerBuilder = playerBuilder;
        }

        public Guid Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public RoomStatus Status { get; set; }
        public GameStatus GameStatus { get; set; }
        public Guid CurrentPlayerId { get; set; }

        public CreateRoomResponseDto Enter(CreateRoomRequestDto requestDto)
        {
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
            var enemyMap = DoMove(playerId, dto.X, dto.Y);

            return new FireResponseDto
            {
                EnemyMap = enemyMap
            };
        }

        public GetRoomStatusResponseDto GetStatus()
        {
            return new GetRoomStatusResponseDto
            {
                PlayerId = Player1.Id,
                RoomId = Id,
                RoomStatus = Status,
                AnotherPlayerName = Player2?.Name
            };
        }

        public GetGameStatusResponseDto GetGameStatus(Guid playerId)
        {
            var gameStatus = GameStatus;

            return new GetGameStatusResponseDto
            {
                YourChoiceTimeout = gameStatus == GameStatus.YourChoice ? TimeSpan.FromMinutes(1) : TimeSpan.Zero,
                MyMap = Player1.Id == playerId ? Player1.OwnMap : Player2.OwnMap,
                GameStatus = gameStatus,
                FinishReason = gameStatus == GameStatus.Finish
                    ? CurrentPlayerId == playerId
                        ? FinishReason.Winner
                        : FinishReason.Lost
                    : (FinishReason?) null
            };
        }

        public Map DoMove(Guid playerId, int x, int y)
        {
            var enemyPlayer = Player1.Id == playerId ? Player2 : Player1;
            var moveResult = enemyPlayer.ProcessEnemyMove(x, y) == CellStatus.EngagedByShipFired;
            CurrentPlayerId = moveResult
                ? playerId
                : enemyPlayer.Id;
            GameStatus = GameStatus.Finish;
            foreach (var cell in enemyPlayer.OwnMap.Cells)
            {
                if (cell.Status == CellStatus.EngagedByShip)
                {
                    GameStatus = moveResult ? GameStatus.YourChoice : GameStatus.PendingForFriendChoice;
                    break;
                }
            }

            return enemyPlayer.OwnMap;
        }
    }
}