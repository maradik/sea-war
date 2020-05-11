namespace Backend.Models
{
    public enum CellStatus
    {
        Empty = 0,
        EngagedByShip = 1,
        ShipNeighbour = 2,
        EmptyFired = 3,
        EngagedByShipFired = 4
    }
}