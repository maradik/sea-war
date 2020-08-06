using Backend.Managers;
using Backend.Models;

namespace Backend.Controllers.v1.Dto
{
    public static class ConverterExtensions
    {
        public static CreateRoomResponseDto ToDto(this EnterOrCreateRoomResult model) =>
            new CreateRoomResponseDto
            {
                PlayerId = model.PlayerId,
                RoomId = model.RoomId,
                RoomStatus = model.RoomStatus,
                AnotherPlayerName = model.AnotherPlayerName
            };

        public static GetRoomStatusResponseDto ToDto(this GetRoomStatusResult model) =>
            new GetRoomStatusResponseDto
            {
                PlayerId = model.PlayerId,
                RoomId = model.RoomId,
                RoomStatus = model.RoomStatus,
                AnotherPlayerName = model.AnotherPlayerName
            };

        public static GetGameStatusResponseDto ToDto(this Game model) =>
            new GetGameStatusResponseDto
            {
                GameStatus = model.GameStatus,
                FinishReason = model.FinishReason,
                YourChoiceTimeout = model.YourChoiceTimeout,
                MyMap = model.MyMap.ToMapDto()
            };

        public static FireResponseDto ToDto(this FireResponse model) =>
            new FireResponseDto
            {
                EnemyMap = model.EnemyMap.ToMapForEnemyDto()
            };
    }
}