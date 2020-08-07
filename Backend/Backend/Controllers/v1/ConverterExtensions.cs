using System;
using Backend.Models;
using Integration.Dtos.v1;

namespace Backend.Controllers.v1
{
    public static class ConverterExtensions
    {
        public static CreateRoomResponseDto ToDto(this EnterOrCreateRoomResult model) =>
            new CreateRoomResponseDto
            {
                PlayerId = model.PlayerId,
                RoomId = model.RoomId,
                RoomStatus = model.RoomStatus.ToDto(),
                AnotherPlayerName = model.AnotherPlayerName
            };

        public static GetGameStatusResponseDto ToDto(this Game model) =>
            new GetGameStatusResponseDto
            {
                GameStatus = model.GameStatus.ToDto(),
                FinishReason = model.FinishReason?.ToDto(),
                YourChoiceTimeout = model.YourChoiceTimeout,
                MyMap = model.MyMap.ToMapDto()
            };

        public static FireResponseDto ToDto(this FireResponse model) =>
            new FireResponseDto
            {
                EnemyMap = model.EnemyMap.ToMapForEnemyDto()
            };

        public static FinishReasonDto ToDto(this FinishReason model) =>
            model switch
            {
                FinishReason.ConnectionLost => FinishReasonDto.ConnectionLost,
                FinishReason.Winner => FinishReasonDto.Winner,
                FinishReason.Lost => FinishReasonDto.Lost,
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };

        public static GameStatusDto ToDto(this GameStatus model) =>
            model switch
            {
                GameStatus.YourChoice => GameStatusDto.YourChoice,
                GameStatus.PendingForFriendChoice => GameStatusDto.PendingForFriendChoice,
                GameStatus.Finish => GameStatusDto.Finish,
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };

        public static RoomStatusDto ToDto(this RoomStatus model) =>
            model switch
            {
                RoomStatus.EmptyRoom => RoomStatusDto.EmptyRoom,
                RoomStatus.NotReady => RoomStatusDto.NotReady,
                RoomStatus.Ready => RoomStatusDto.Ready,
                RoomStatus.Orphaned => RoomStatusDto.Orphaned,
                RoomStatus.Finished => RoomStatusDto.Finished,
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };

        public static CellStatusDto ToDto(this CellStatus model) =>
            model switch
            {
                CellStatus.Empty => CellStatusDto.Empty,
                CellStatus.EngagedByShip => CellStatusDto.EngagedByShip,
                CellStatus.EmptyFired => CellStatusDto.EmptyFired,
                CellStatus.EngagedByShipFired => CellStatusDto.EngagedByShipFired,
                CellStatus.ShipNeighbour => CellStatusDto.ShipNeighbour,
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };

        public static MapDto ToMapDto(this Map map)
        {
            var mapDto = new MapDto();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    mapDto.Cells[i, j] = new CellDto {X = i, Y = j, Status = map.Cells[i, j].Status.ToDto()};
                }
            }

            return mapDto;
        }

        public static MapForEnemyDto ToMapForEnemyDto(this Map map)
        {
            var mapDto = new MapForEnemyDto();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    mapDto.Cells[i, j] = new CellForEnemyDto
                    {
                        Status = map.Cells[i, j].Status switch
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