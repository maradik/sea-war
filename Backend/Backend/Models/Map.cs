using System.Collections.Generic;
using System.Linq;
using Backend.Controllers.v1.Dto;

namespace Backend.Models
{
    public class Map
    {
        public Cell[,] Cells { get; set; }
        public Ship[] Ships { get; set; }

        public bool IsEmpty(int x, int y) =>
            Cells[x, y].Status == CellStatus.Empty;

        public Cell[] GetCellNeighbours(Cell cell)
        {
            var result = new List<Cell>();
            for (var i = -1; i <= 1; ++i)
            {
                for (var j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0)
                        continue;
                    if (cell.X + i >= 0 && cell.X + i < 10 && cell.Y + j >= 0 && cell.Y + j < 10)
                        result.Add(Cells[cell.X + i, cell.Y + j]);
                }
            }

            return result.ToArray();
        }

        public bool HasAliveShip() =>
            Ships.Any(ship => ship.Status == ShipStatus.Alive);

        public bool HasShip(int x, int y) =>
            Cells[x, y].Status == CellStatus.EngagedByShip;

        public void Fire(int x, int y) =>
            Cells[x, y].Status = Cells[x, y].Status == CellStatus.Empty ? CellStatus.EmptyFired : CellStatus.EngagedByShipFired;

        public Ship GetShip(int x, int y) =>
            Ships.Single(ship => ship.Cells.Any(cell => cell.X == x && cell.Y == y));

        public void DismissShipNeighbours(int x, int y)
        {
            var ship = GetShip(x, y);
            var allNeighbours = ship.Cells
                                    .SelectMany(GetCellNeighbours)
                                    .Where(neighbour => !ship.ContainsCell(neighbour.X, neighbour.Y));

            foreach (var neighbour in allNeighbours)
            {
                Cells[neighbour.X, neighbour.Y].Status = CellStatus.ShipNeighbour;
            }
        }

        public bool TryAddShip(Ship ship)
        {
            foreach (var cell in ship.Cells)
            {
                if (Cells[cell.X, cell.Y].Status != CellStatus.Empty)
                    return false;
                for (var i = -1; i <= 1; ++i)
                {
                    for (var j = -1; j <= 1; ++j)
                    {
                        if (i == 0 && j == 0)
                            continue;
                        if (cell.X + i >= 0 && cell.X + i < 10 && cell.Y + j >= 0 && cell.Y + j < 10)
                        {
                            if (Cells[cell.X + i, cell.Y + j].Status != CellStatus.Empty)
                                return false;
                        }
                    }
                }
            }

            foreach (var cell in ship.Cells)
            {
                Cells[cell.X, cell.Y].Status = CellStatus.EngagedByShip;
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
                    mapDto.Cells[i, j] = new Cell{X = i, Y = j, Status =  Cells[i, j].Status};
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
                    mapDto.Cells[i, j] = new CellForEnemyDto
                    {
                        Status = Cells[i, j].Status switch
                        {
                            CellStatus.EmptyFired => CellForEnemyDtoStatus.Missed,
                            CellStatus.EngagedByShipFired => CellForEnemyDtoStatus.Damaged,
                            CellStatus.ShipNeighbour => CellForEnemyDtoStatus.ShipNeighbour,
                            _ => CellForEnemyDtoStatus.Unknown
                        }
                    };
                }
            }

            return mapDto;
        }
    }
}