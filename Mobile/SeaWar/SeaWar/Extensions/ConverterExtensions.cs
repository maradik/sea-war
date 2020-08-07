using System;
using Integration.Dtos.v2;
using SeaWar.ViewModels;
using Cell = SeaWar.ViewModels.Cell;
using FinishReason = SeaWar.DomainModels.FinishReason;
using Map = SeaWar.ViewModels.Map;

namespace SeaWar.Extensions
{
    public static class ConverterExtensions
    {
        public static Map ToModel(this MapDto dto)
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
        
        public static Map ToModel(this MapForEnemyDto dto)
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

        public static Cell ToModel(this CellDto dto) =>
            new Cell
            {
                Status = dto.Status.ToModel()
            };


        public static Cell ToModel(this CellForEnemyDto dto) =>
            new Cell
            {
                Status = dto.Status.ToModel()
            };

        public static CellStatus ToModel(this CellStatusDto dto) =>
            dto switch
            {
                CellStatusDto.Empty => CellStatus.Empty,
                CellStatusDto.EngagedByShip => CellStatus.Filled,
                CellStatusDto.EmptyFired => CellStatus.Missed,
                CellStatusDto.EngagedByShipFired => CellStatus.Damaged,
                CellStatusDto.ShipNeighbour => CellStatus.Missed,
                _ => throw new ArgumentException(nameof(dto))
            };
        
        public static CellStatus ToModel(this CellForEnemyDtoStatus dto) =>
            dto switch
            {
                CellForEnemyDtoStatus.Unknown => CellStatus.Empty,
                CellForEnemyDtoStatus.Missed => CellStatus.Missed,
                CellForEnemyDtoStatus.Damaged => CellStatus.Damaged,
                CellForEnemyDtoStatus.ShipNeighbour => CellStatus.Missed,
                _ => throw new ArgumentException()
            };

        public static FinishReason ToModel(this FinishReasonDto? dto) =>
            dto switch
            {
                FinishReasonDto.ConnectionLost => FinishReason.OpponentConnectionLost,
                FinishReasonDto.Winner => FinishReason.Winner,
                FinishReasonDto.Lost => FinishReason.Lost,
                _ => throw new Exception()
            };
    }
}