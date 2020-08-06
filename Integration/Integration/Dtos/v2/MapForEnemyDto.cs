namespace Integration.Dtos.v2
{
    public class MapForEnemyDto
    {
        public CellForEnemyDto[,] Cells { get; set; } = new CellForEnemyDto[10, 10];
    }
}