using System.Linq;
using Backend.Controllers.Dto;

namespace Backend.Models
{
    public class Map
    {
        public Cell[,] Cells { get; set; }
        public Ship[] Ships { get; set; }

        public bool IsEmpty(int x, int y) =>
            Cells[y, x].Status == CellStatus.Empty;

        public bool HasAliveShip() =>
            Ships.Any(ship => ship.Status == ShipStatus.Alive);

        public bool HasShip(int x, int y) =>
            Cells[y, x].Status == CellStatus.EngagedByShip;

        public Ship GetShip(int x, int y) =>
            Ships.First(ship => ship.Cells.Any(cell => cell.X == x && cell.Y == y));

        public void DismissShipNeighbours(int x, int y)
        {
            var ship = GetShip(x, y);
            foreach (var cell in ship.Cells)
            {
                for (var i = -1; i <= 1; ++i)
                {
                    for (var j = -1; j <= 1; ++j)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (Cells[cell.Y + j, cell.X + i].Status == CellStatus.Empty)
                            Cells[cell.Y + j, cell.X + i].Status = CellStatus.ShipNeighbour;
                    }
                }
            }
        }

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
                        if (cell.X + i >= 0 && cell.X + i < 10 && cell.Y + j >= 0 && cell.Y + j < 10)
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

            Ships = Ships.Append(ship).ToArray();

            return true;
        }

        public MapDto ToMapDto()
        {
            var mapDto = new MapDto();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    mapDto.Cells[i, j].Status = Cells[i, j].Status;
                }
            }

            return mapDto;
        }

        public MapForEnemyDto ToMapForEnemyDto()
        {
            var mapDto = new MapForEnemyDto();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    mapDto.Cells[i, j].Status = Cells[i, j].Status switch
                    {
                        CellStatus.EmptyFired => CellForEnemyDtoStatus.Missed,
                        CellStatus.EngagedByShipFired => CellForEnemyDtoStatus.Damaged,
                        _ => CellForEnemyDtoStatus.Unknown
                    };
                }
            }

            return mapDto;
        }
    }
}