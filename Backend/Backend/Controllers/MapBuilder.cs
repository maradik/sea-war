using System;
using Backend.Models;

namespace Backend.Controllers
{
    public class MapBuilder
    {
        public Map Build()
        {
            var cells = new Cell[10, 10];
            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    cells[i, j] = new Cell
                    {
                        Status = CellStatus.Empty
                    };
                }
            }

            return new Map
            {
                Cells = cells,
                Ships = Array.Empty<Ship>()
            };
        }
    }
}