namespace Backend.Controllers.v2.Dto
{
    public class MapDto
    {
        public CellDto[,] Cells { get; set; } = new CellDto[10, 10];
    }
}