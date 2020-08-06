namespace Backend.Controllers.v2.Dto
{
    public enum CellStatusDto
    {
        Empty = 0,
        EngagedByShip = 1,
        EmptyFired = 2,
        EngagedByShipFired = 3,
        ShipNeighbour = 4
    }
}