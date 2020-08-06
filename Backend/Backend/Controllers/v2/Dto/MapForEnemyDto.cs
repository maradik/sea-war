namespace Backend.Controllers.v2.Dto
{
    public class MapForEnemyDto
    {
        public CellForEnemyDto[,] Cells { get; set; } = new CellForEnemyDto[10, 10];
    }
}