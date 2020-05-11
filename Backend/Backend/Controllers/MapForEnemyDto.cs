namespace Backend.Controllers
{
    public class MapForEnemyDto
    {
        public CellForEnemyDto[,] Cells { get; set; } = new CellForEnemyDto[10, 10];
    }
    
    public class CellForEnemyDto
    {
        public CellForEnemyDtoStatus Status { get; set; }
    }

    public enum CellForEnemyDtoStatus
    {
        Empty = 0,
        Missed = 1,
        Damaged = 2
    }
}