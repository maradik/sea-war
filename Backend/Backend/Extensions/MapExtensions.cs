using System.Linq;
using Backend.Models;

namespace Backend.Extensions
{
    public static class MapExtensions
    {
        public static Map ToEnemyMap(this Map map) =>
            new Map
            {
                Cells = map.Cells.ToEnemyCells()
            };

        private static Cell[,] ToEnemyCells(this Cell[,] cells)
        {
            var enemyCells = new Cell[cells.GetLength(0), cells.GetLength(1)];
            foreach (var cell in cells.Cast<Cell>())
            {
                enemyCells[cell.X, cell.Y] = cell.ToEnemyCell();
            }

            return enemyCells;
        }

        private static Cell ToEnemyCell(this Cell cell) =>
            new Cell
            {
                X = cell.X,
                Y = cell.Y,
                Status = cell.Status != CellStatus.EngagedByShip ? cell.Status : CellStatus.Empty
            };
    }
}