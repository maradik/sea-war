using Backend.Models;
using NUnit.Framework;

namespace Backend.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void Test()
        {
            var map = new Map();
            map.Cells = new Cell[10, 10];
            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    map.Cells[i, j] = new Cell {X = i, Y = j};
                }
            }
            var result = map.GetCellNeighbours(new Cell {X = 1, Y = 1});
        }
    }
}