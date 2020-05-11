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

        public bool ContainsCell(int x, int y)
        {
            return Cells.Any(cell => cell.X == x && cell.Y == y);
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