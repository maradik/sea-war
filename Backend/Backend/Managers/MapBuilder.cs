using System;
using System.Collections.Generic;
using Backend.Models;

namespace Backend.Managers
{
    public class MapBuilder
    {
        public Map Build()
        {
            var map = new Map
            {
                Cells = GetEmptyCells(),
                Ships = Array.Empty<Ship>()
            };
            GenerateShipPositions(map);

            return map;
        }

        private static Cell[,] GetEmptyCells()
        {
            var cells = new Cell[10, 10];
            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    cells[i, j] = new Cell
                    {
                        X = j,
                        Y = i,
                        Status = CellStatus.Empty
                    };
                }
            }

            return cells;
        }

        private void GenerateShipPositions(Map map)
        {
            var sizes = new[] {4, 3, 3, 2, 2, 2, 1, 1, 1, 1};

            var possiblePositions = new Dictionary<int, List<Ship>>
            {
                [1] = GenerateAllPositions(1),
                [2] = GenerateAllPositions(2),
                [3] = GenerateAllPositions(3),
                [4] = GenerateAllPositions(4)
            };

            for (var i = 0; i < 10; ++i)
            {
                var size = sizes[i];
                var allPossiblePositions = possiblePositions[size];
                foreach (var position in allPossiblePositions)
                {
                    if (map.TryAddShip(position))
                    {
                        break;
                    }
                }
            }
        }

        private List<Ship> GenerateAllPositions(int size)
        {
            var result = new List<Ship>();
            for (var i = 0; i <= 10 - size; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    var cells = new List<Cell>();
                    for (var k = 0; k < size; ++k)
                    {
                        cells.Add(new Cell {X = i + k, Y = j, Status = CellStatus.EngagedByShip});
                    }
                    result.Add(new Ship {Cells = cells.ToArray()});
                }
            }
            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j <= 10 - size; ++j)
                {
                    var cells = new List<Cell>();
                    for (var k = 0; k < size; ++k)
                    {
                        cells.Add(new Cell {X = i, Y = j + k, Status = CellStatus.EngagedByShip});
                    }
                    result.Add(new Ship {Cells = cells.ToArray()});
                }
            }
            return result;
        }
    }
}