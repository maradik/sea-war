using SeaWar.DomainModels;

namespace SeaWar.Client.Contracts
{
    public class EnemyMap
    {
        public static EnemyMap Empty => new EnemyMap {Cells = CreateCells()};
        public EnemyCell[,] Cells { get; set; }

        private static EnemyCell[,] CreateCells()
        {
            var cells = new EnemyCell[GameModel.MapHorizontalSize, GameModel.MapVerticalSize];
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    cells[x, y] = new EnemyCell();
                }
            }

            return cells;
        }
    }
}