namespace Backend.Controllers.Dto
{
    public class MapForEnemyDto
    {
        public CellForEnemyDto[,] Cells { get; set; } = new CellForEnemyDto[10, 10];
    }
}