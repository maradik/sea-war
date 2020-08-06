namespace Backend.Controllers.v1.Dto
{
    public class MapDto
    {
        public CellDto[,] Cells { get; set; } = new CellDto[10, 10];
    }
}