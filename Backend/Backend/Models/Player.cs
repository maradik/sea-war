using System;

namespace Backend.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Map OwnMap { get; set; }
        public Map EnemyMap { get; set; }

        public CellStatus ProcessEnemyMove(int x, int y)
        {
            if (OwnMap.Cells[y, x].Status == CellStatus.Empty || OwnMap.Cells[y, x].Status == CellStatus.ShipNeighbour)
            {
                OwnMap.Cells[y, x].Status = CellStatus.EmptyFired;
                return CellStatus.EmptyFired;
            }

            if (OwnMap.Cells[y, x].Status == CellStatus.EngagedByShip)
            {
                OwnMap.Cells[y, x].Status = CellStatus.EngagedByShipFired;
                return CellStatus.EngagedByShipFired;
            }

            throw new ArgumentException("Incorrect move");
        }
    }
}