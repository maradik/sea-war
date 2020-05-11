using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Backend.Models
{
    public class Ship
    {
        public Cell[] Cells { get; set; }

        public void Damage(int x, int y)
        {
            Cells.Single(cell => cell.X == x && cell.Y == y).Status = CellStatus.EngagedByShipFired;
        }

        public ShipStatus Status => Cells.Any(cell => cell.Status == CellStatus.EngagedByShip)
            ? ShipStatus.Alive
            : ShipStatus.Killed;
    }

    public enum ShipStatus
    {
        Alive,
        Killed
    }
}