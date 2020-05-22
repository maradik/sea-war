using System;
using System.Collections.Generic;
using SeaWar.DomainModels;

namespace SeaWar.ViewModels
{
    public class Map
    {
        public static Map Empty => new Map {Cells = CreateCells()};

        public Cell[,] Cells { get; set; }

        public CellPosition[] GetCellPositionsWithStatus(CellStatus status)
        {
            var result = new List<CellPosition>();
            
            for (int x = 0; x < GameModel.MapHorizontalSize; x++)
            {
                for (int y = 0; y < GameModel.MapVerticalSize; y++)
                {
                    if (Cells[x, y].Status == status)
                    {
                        result.Add(new CellPosition(x, y));
                    }
                }
            }

            return result.ToArray();
        }

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