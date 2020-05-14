﻿using System;
using SeaWar.Client.Contracts;
using SeaWar.ViewModels;
using Cell = SeaWar.ViewModels.Cell;
using FinishReason = SeaWar.DomainModels.FinishReason;
using Map = SeaWar.ViewModels.Map;

namespace SeaWar.Extensions
{
    public static class ConverterExtensions
    {
        public static Map ToModel(this Client.Contracts.Map dto)
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
        
        public static Map ToModel(this EnemyMap dto)
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

        public static Cell ToModel(this Client.Contracts.Cell dto) =>
            new Cell
            {
                Status = dto.Status.ToModel()
            };


        public static Cell ToModel(this EnemyCell dto) =>
            new Cell
            {
                Status = dto.Status.ToModel()
            };

        public static CellStatus ToModel(this Client.Contracts.Cell.CellStatus dto) =>
            dto switch
            {
                Client.Contracts.Cell.CellStatus.Empty => CellStatus.Empty,
                Client.Contracts.Cell.CellStatus.EngagedByShip => CellStatus.Filled,
                Client.Contracts.Cell.CellStatus.EmptyFired => CellStatus.Missed,
                Client.Contracts.Cell.CellStatus.EngagedByShipFired => CellStatus.Damaged,
                Client.Contracts.Cell.CellStatus.ShipNeighbour => CellStatus.Missed,
                _ => throw new ArgumentException(nameof(dto))
            };
        
        public static CellStatus ToModel(this EnemyCellStatus dto) =>
            dto switch
            {
                EnemyCellStatus.Unknown => CellStatus.Empty,
                EnemyCellStatus.Missed => CellStatus.Missed,
                EnemyCellStatus.Damaged => CellStatus.Damaged,
                EnemyCellStatus.ShipNeighbour => CellStatus.Missed,
                _ => throw new ArgumentException()
            };

        public static FinishReason ToModel(this Client.Contracts.FinishReason? dto) =>
            dto switch
            {
                Client.Contracts.FinishReason.ConnectionLost => FinishReason.OpponentConnectionLost,
                Client.Contracts.FinishReason.Winner => FinishReason.Winner,
                Client.Contracts.FinishReason.Lost => FinishReason.Lost,
                _ => throw new Exception()
            };
    }
}