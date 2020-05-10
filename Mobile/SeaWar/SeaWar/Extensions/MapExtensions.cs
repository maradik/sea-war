using System;
using SeaWar.ViewModels;

namespace SeaWar.Extensions
{
    public static class MapExtensions
    {
        public static ViewModels.Map ToModel(this Contracts.Map dto)
        {
            var xLength = dto.Cells.GetLength(0);
            var yLength = dto.Cells.GetLength(1);

            var domainModel = new Map
            {
                Cells = new Cell[xLength, yLength]
            };

            for (var x = 0; x < xLength; x++)
            {
                for (var y = 0; y < yLength; y++)
                {
                    domainModel.Cells[x, y] = dto.Cells[x, y].ToModel();
                }
            }

            return domainModel;
        }
        
        public static Cell ToModel(this Contracts.Cell dto) =>
            new Cell
            {
                Status = dto.Status.ToModel()
            };

        public static CellStatus ToModel(this Contracts.Cell.CellStatus dto) =>
            dto switch
            {
                Contracts.Cell.CellStatus.Empty => CellStatus.Empty,
                Contracts.Cell.CellStatus.EngagedByShip => CellStatus.Filled,
                Contracts.Cell.CellStatus.EmptyFired => CellStatus.Missed,
                Contracts.Cell.CellStatus.EngagedByShipFired => CellStatus.Damaged,
                _ => throw new ArgumentException(nameof(dto))
            };
    }
}