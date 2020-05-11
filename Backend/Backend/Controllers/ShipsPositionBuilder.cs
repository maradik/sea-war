using System.Collections.Generic;
using Backend.Models;

namespace Backend.Controllers
{
    public class ShipsPositionBuilder
    {
        private readonly MapBuilder mapBuilder;

        public ShipsPositionBuilder(MapBuilder mapBuilder) =>
            this.mapBuilder = mapBuilder;

        public Map Build()
        {
            var sizes = new[] {4, 3, 3, 2, 2, 2, 1, 1, 1, 1};
            var ships = new List<Ship>();

            var possiblePositions = new Dictionary<int, List<Ship>>
            {
                [1] = GenerateAllPositions(1),
                [2] = GenerateAllPositions(2),
                [3] = GenerateAllPositions(3),
                [4] = GenerateAllPositions(4)
            };

            var map = mapBuilder.Build();

            for (var i = 0; i < 10; ++i)
            {
                var size = sizes[i];
                var allPossiblePositions = possiblePositions[size];
                foreach (var position in allPossiblePositions)
                {
                    if (map.TryAddShip(position))
                    {
                        ships.Add(position);
                        break;
                    }
                }
            }

            return map;
        }

        private List<Ship> GenerateAllPositions(int size)
        {
            var result = new List<Ship>();
            for (var i = 0; i <= 10 - size; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    var cells = new List<Point>();
                    for (var k = 0; k < size; ++k)
                    {
                        cells.Add(new Point {X = i + k, Y = j});
                    }
                    result.Add(new Ship() {Cells = cells.ToArray()});
                }
            }
            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j <= 10 - size; ++j)
                {
                    var cells = new List<Point>();
                    for (var k = 0; k < size; ++k)
                    {
                        cells.Add(new Point {X = i, Y = j + k});
                    }
                    result.Add(new Ship() {Cells = cells.ToArray()});
                }
            }
            return result;
        }
    }
}