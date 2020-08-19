using System;
using System.Linq;

namespace Backend.Models
{
    public class Bot
    {
        private static readonly Random random = new Random();
        private readonly Room room;

        public Bot(Guid id, string name, Room room)
        {
            Id = id;
            Name = name;
            this.room = room;
        }

        public Guid Id { get; }
        private string Name { get; }
        private Map EnemyMap { get; set; }
        private Cell LastEngagedByShipFiredCell { get; set; }

        public bool TryPlay()
        {
            if (!room.IsActive)
            {
                return false;
            }

            try
            {
                if (room.IsOpened)
                {
                    room.Enter(Id, Name);
                }

                var game = room.GetGameFor(Id);
                if (game.GameStatus == GameStatus.YourChoice)
                {
                    Fire();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void Fire()
        {
            var (x, y) = GetFireCoordinates();
            var fireResponse = room.Fire(x, y, Id);
            EnemyMap = fireResponse.EnemyMap;

            var firedCell = EnemyMap.Cells[x, y];
            if (firedCell.Status == CellStatus.EngagedByShipFired)
            {
                LastEngagedByShipFiredCell = firedCell;
            }
        }

        private (int x, int y) GetFireCoordinates()
        {
            if (LastEngagedByShipFiredCell != default)
            {
                if (TryGetEmptyEnemyCellIfNeighborFired(LastEngagedByShipFiredCell, out var emptyCell))
                {
                    return (emptyCell.X, emptyCell.Y);
                }

                if (TryGetNeighborEmptyEnemyCell(LastEngagedByShipFiredCell, out emptyCell))
                {
                    return (emptyCell.X, emptyCell.Y);
                }

                LastEngagedByShipFiredCell = default;
            }

            return GetRandomFireCoordinates();
        }

        private bool TryGetEmptyEnemyCellIfNeighborFired(Cell cell, out Cell emptyCell)
        {
            if (TryGetEmptyEnemyCellByPreviousFireDirection(cell, 0, -1, out emptyCell) ||
                TryGetEmptyEnemyCellByPreviousFireDirection(cell, 0, 1, out emptyCell) ||
                TryGetEmptyEnemyCellByPreviousFireDirection(cell, -1, 0, out emptyCell) ||
                TryGetEmptyEnemyCellByPreviousFireDirection(cell, 1, 0, out emptyCell))
            {
                return true;
            }

            return false;
        }

        private bool TryGetNeighborEmptyEnemyCell(Cell cell, out Cell emptyCell)
        {
            if (TryGetEnemyCellByOffset(cell, 0, -1, out emptyCell) && emptyCell.Status == CellStatus.Empty ||
                TryGetEnemyCellByOffset(cell, 0, 1, out emptyCell) && emptyCell.Status == CellStatus.Empty ||
                TryGetEnemyCellByOffset(cell, -1, 0, out emptyCell) && emptyCell.Status == CellStatus.Empty ||
                TryGetEnemyCellByOffset(cell, 1, 0, out emptyCell) && emptyCell.Status == CellStatus.Empty)
            {
                return true;
            }

            return false;
        }

        private bool TryGetEmptyEnemyCellByPreviousFireDirection(Cell cell, int offsetX, int offsetY, out Cell emptyCell)
        {
            if (!TryGetEnemyCellByOffset(cell, offsetX, offsetY, out var neighborCell) || neighborCell.Status != CellStatus.EngagedByShipFired)
            {
                emptyCell = default;
                return false;
            }

            var isTurned = false;
            while (true)
            {
                switch (neighborCell?.Status)
                {
                    case CellStatus.Empty:
                        emptyCell = neighborCell;
                        return true;
                    case CellStatus.ShipNeighbour:
                    case CellStatus.EmptyFired:
                    case null:
                        if (!isTurned)
                        {
                            offsetX *= -1;
                            offsetY *= -1;
                            isTurned = true;
                        }
                        else
                        {
                            emptyCell = default;
                            return false;
                        }

                        break;
                    case CellStatus.EngagedByShipFired:
                        cell = neighborCell;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(neighborCell.Status), neighborCell.Status, null);
                }

                TryGetEnemyCellByOffset(cell, offsetX, offsetY, out neighborCell);
            }
        }

        private (int x, int y) GetRandomFireCoordinates()
        {
            if (EnemyMap == null)
            {
                return (random.Next(10), random.Next(10));
            }

            var emptyCells = EnemyMap.Cells.Cast<Cell>().Where(x => x.Status == CellStatus.Empty).ToArray();
            var chosenCell = emptyCells[random.Next(emptyCells.Length)];
            return (chosenCell.X, chosenCell.Y);
        }

        private bool TryGetEnemyCellByOffset(Cell cell, int offsetX, int offsetY, out Cell resultCell)
        {
            var x = cell.X + offsetX;
            var y = cell.Y + offsetY;

            if (x >= 0 && x < EnemyMap.Cells.GetLength(0) &&
                y >= 0 && y < EnemyMap.Cells.GetLength(1))
            {
                resultCell = EnemyMap.Cells[x, y];
                return true;
            }

            resultCell = default;
            return false;
        }
    }
}