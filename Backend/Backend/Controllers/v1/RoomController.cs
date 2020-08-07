using System;
using Backend.Managers;
using Integration.Dtos.v1;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.v1
{
    [ApiController]
    [Route("room")]
    public class RoomController : ControllerBase
    {
        private readonly RoomManager roomManager;

        public RoomController(RoomManager roomManager) =>
            this.roomManager = roomManager;

        [HttpPost]
        [Route("Create")]
        public CreateRoomResponseDto Create([FromBody] CreateRoomRequestDto requestDto) =>
            roomManager.EnterOrCreateRoom(requestDto.PlayerId ?? Guid.NewGuid(), requestDto.PlayerName).ToDto();

        [HttpGet]
        [Route("{roomId}/Game/GetStatus")]
        public GetGameStatusResponseDto GetGameStatus([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            roomManager.GetGame(roomId, playerId).ToDto();

        [HttpPost]
        [Route("{roomId}/Game/Fire")]
        public FireResponseDto Fire([FromBody] FireRequestDto dto, [FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            roomManager.Fire(dto.X, dto.Y, roomId, playerId).ToDto();

        [HttpGet]
        [Route("{roomId}/getStatus")]
        public GetRoomStatusResponseDto GetStatus([FromRoute] Guid roomId, [FromQuery] Guid playerId)
        {
            var room = roomManager.GetRoom(roomId);

            return new GetRoomStatusResponseDto
            {
                PlayerId = playerId,
                RoomId = room.Id,
                RoomStatus = room.Status.ToDto(),
                AnotherPlayerName = room.GetEnemyPlayerFor(playerId)?.Name
            };
        }
    }
}