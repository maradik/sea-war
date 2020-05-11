namespace Backend.Controllers
{
    public class MapDto
    {
        public CellDto[,] Cells { get; set; } = new CellDto[10, 10];
    }
}