namespace Backend.Controllers.v1.Dto
{
    public class CellDto
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CellStatusDto Status { get; set; }
    }
}