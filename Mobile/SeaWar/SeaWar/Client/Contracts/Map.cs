using SeaWar.DomainModels;

namespace SeaWar.Client.Contracts
{
    public class Map
    {
        public static Map Empty => new Map {Cells = CreateCells()};

        public Cell[,] Cells { get; set; }

        private static Cell[,] CreateCells()
        {
            var cells = new Cell[GameModel.MapHorizontalSize, GameModel.MapVerticalSize];
            for (var x = 0; x < cells.GetLength(0); x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    cells[x, y] = new Cell();
                }
            }

            return cells;
        }
    }
}