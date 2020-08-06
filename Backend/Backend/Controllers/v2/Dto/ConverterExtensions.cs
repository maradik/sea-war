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
                Status = model.GameStatus,
                FinishReason = model.FinishReason,
                YourChoiceTimeout = model.YourChoiceTimeout,
                MyMap = model.MyMap.ToMapDto()
            };
    }
}