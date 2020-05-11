using System;
using Backend.Controllers;

namespace Backend.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public RoomStatus Status { get; set; }
        public GameStatus GameStatus { get; set; }
        public Guid CurrentPlayerId { get; set; }

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

            return enemyPlayer.EnemyMap;
        }
    }
}