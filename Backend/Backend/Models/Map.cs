using Backend.Controllers;

namespace Backend.Models
{
    public class Map
    {
        public Cell[,] Cells { get; set; }

        public bool TryAddShip(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                if (Cells[cell.Y, cell.X].Status != CellStatus.Empty)
                    return false;
                for (var i = -1; i <= 1; ++i)
                {
                    for (var j = -1; j <= 1; ++j)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (cell.X + i >= 0 && cell.X + i < 10 && cell.Y + j >= 0 && cell.Y < 10)
                        {
                            if (Cells[cell.Y + j, cell.X + i].Status != CellStatus.Empty)
                                return false;
                        }
                    }
                }
            }

            foreach (var cell in ship.Cells)
            {
                Cells[cell.Y, cell.X].Status = CellStatus.EngagedByShip;
            }

            return true;
        }
    }
}