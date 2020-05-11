using System;
using System.Collections.Generic;
using Backend.Controllers;
using Backend.Models;
using Microsoft.Extensions.Hosting.Internal;
using NUnit.Framework;

namespace Backend.Tests
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void Test()
        {
            var result = new ShipsPositionBuilder(new MapBuilder()).Build();
            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    if (result.Cells[i, j].Status == CellStatus.Empty)
                        Console.Write("* ");
                    else if (result.Cells[i, j].Status == CellStatus.ShipNeighbour)
                        Console.Write("- ");
                    else if (result.Cells[i, j].Status == CellStatus.EngagedByShip)
                        Console.Write("+ ");
                }

                Console.WriteLine();
            }
        }
    }
}