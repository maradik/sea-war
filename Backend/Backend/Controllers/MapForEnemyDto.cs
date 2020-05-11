namespace Backend.Controllers
{
    public class MapForEnemyDto
    {
        public CellForEnemyDto[,] Cells { get; set; } = new CellForEnemyDto[10, 10];
    }
}