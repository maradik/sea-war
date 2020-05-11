namespace Backend.Controllers
{
    public class MapDto
    {
        public CellDto[,] Cells { get; set; } = new CellDto[10, 10];
    }

    public class CellDto
    {
        public CellDtoStatus Status { get; set; }
    }

    public enum CellDtoStatus
    {
        Empty = 0,
        EngagedByShip = 1,
        ShipNeighbour = 2,
        EmptyFired = 3,
        EngagedByShipFired = 4
    }
}