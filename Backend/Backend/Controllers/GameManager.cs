using System;

namespace Backend.Controllers
{
    public class GameManager
    {
        private readonly RoomManager roomManager;

        public GameManager(RoomManager roomManager) =>
            this.roomManager = roomManager;

        public FireResponseDto Fire(FireRequestDto dto, Guid roomId, Guid playerId) =>
            roomManager.GetRoom(roomId).Fire(dto, playerId);

        public GetGameStatusResponseDto GetGameStatus(Guid roomId, Guid playerId) =>
            roomManager.GetRoom(roomId).GetGameStatus(playerId);

        public GetRoomStatusResponseDto GetStatus(Guid roomId, Guid playerId) =>
            roomManager.GetRoom(roomId).GetStatus();

        public CreateRoomResponseDto Create(CreateRoomRequestDto requestDto) =>
            roomManager.CreateOrEnterRoom(requestDto);
    }
}