using System;
using System.Linq;
using Backend.Models;

namespace Backend.Controllers.v2.Dto
{
    public static class ConverterExtensions
    {
        public static CreateRoomResponseDto ToDto(this CreateRoomResult model) =>
            new CreateRoomResponseDto
            {
                RoomId = model.RoomId
            };

        public static JoinRoomResponseDto ToDto(this JoinRoomResult model) =>
            new JoinRoomResponseDto
            {
                Success = model.Success
            };

        public static RoomListResponseDto ToDto(this Room[] rooms) =>
            new RoomListResponseDto
            {
                Rooms = rooms.Select(x => x.ToDto()).ToArray()
            };

        public static RoomDto ToDto(this Room room) =>
            new RoomDto
            {
                Id = room.Id,
                Players = new[] {room.Player1?.ToDto(), room.Player2?.ToDto()}.Where(x => x != null).ToArray(),
                Status = room.Status.ToDto()
            };

        public static PlayerDto ToDto(this Player player) =>
            new PlayerDto
            {
                Id = player.Id,
                Name = player.Name
            };

        public static RoomStatusDto ToDto(this RoomStatus roomStatus) =>
            roomStatus switch
            {
                RoomStatus.EmptyRoom => RoomStatusDto.Opened,
                RoomStatus.NotReady => RoomStatusDto.Opened,
                RoomStatus.Ready => RoomStatusDto.Ready,
                _ => throw new ArgumentOutOfRangeException(nameof(roomStatus), roomStatus, null)
            };

        public static FireResponseDto ToDto(this FireResponse model) =>
            new FireResponseDto
            {
                EnemyMap = model.EnemyMap.ToMapForEnemyDto()
            };

        public static GameDto ToDto(this Game model) =>
            new GameDto
            {
                Status = model.GameStatus.ToDto(),
                FinishReason = model.FinishReason?.ToDto(),
                YourChoiceTimeout = model.YourChoiceTimeout,
                MyMap = model.MyMap.ToMapDto()
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
    }
}