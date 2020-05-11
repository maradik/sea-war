namespace Backend.Models
{
    public enum CellStatus
    {
        Empty = 0,
        EngagedByShip = 1,
        EmptyFired = 2,
        EngagedByShipFired = 3,
        ShipNeighbour = 4
    }
}