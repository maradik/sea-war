using Backend.Models;

namespace Backend.Controllers.Dto
{
    public class MapDto
    {
        public Cell[,] Cells { get; set; } = new Cell[10, 10];
    }
}