using System;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
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
            roomManager.Create(requestDto);

        [HttpGet]
        [Route("{roomId}/Game/GetStatus")]
        public GetGameStatusResponseDto GetGameStatus([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            roomManager.GetGameStatus(roomId, playerId);

        [HttpPost]
        [Route("{roomId}/Game/Fire")]
        public FireResponseDto Fire([FromBody] FireRequestDto dto, [FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            roomManager.Fire(dto, roomId, playerId);

        [HttpGet]
        [Route("{roomId}/getStatus")]
        public GetRoomStatusResponseDto GetStatus([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            roomManager.GetStatus(roomId, playerId);
    }
}