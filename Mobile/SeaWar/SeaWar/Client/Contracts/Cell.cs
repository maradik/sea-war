namespace SeaWar.Client.Contracts
{
    public class Cell
    {
        public CellStatus Status { get; set; }

        public enum CellStatus
        {
            Empty = 0,
            EngagedByShip = 1,
            EmptyFired = 2,
            EngagedByShipFired = 3,
            ShipNeighbour = 4
        }
    }
}