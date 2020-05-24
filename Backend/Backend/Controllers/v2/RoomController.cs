using System;
using Backend.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.v2
{
    [ApiController]
    [Route("v2/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly RoomManager roomManager;

        public RoomController(RoomManager roomManager) =>
            this.roomManager = roomManager;

        [HttpGet]
        public GetRoomListResponseDto List([FromQuery] Guid playerId) =>
            throw new NotImplementedException();

        [HttpPost]
        public CreateRoomResponseDto Create([FromBody] CreateRoomRequestDto requestDto, [FromQuery] Guid playerId) =>
            throw new NotImplementedException();
        
        [HttpPost]
        [Route("{roomId}/Join")]
        public JoinRoomResponseDto Join([FromBody] JoinRoomRequestDto requestDto, [FromQuery] Guid playerId) =>
            throw new NotImplementedException();

        [HttpGet]
        [Route("{roomId}")]
        public GetRoomResponseDto Get([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            throw new NotImplementedException();

        [HttpGet]
        [Route("{roomId}/Game")]
        public GetGameResponseDto GetGame([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            throw new NotImplementedException();

        [HttpPost]
        [Route("{roomId}/Game/Fire")]
        public FireResponseDto Fire([FromBody] FireRequestDto dto, [FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            throw new NotImplementedException();
    }

    public class FireRequestDto
    {
    }

    public class FireResponseDto
    {
    }

    public class GetGameResponseDto
    {
    }

    public class GetRoomResponseDto
    {
    }

    public class JoinRoomResponseDto
    {
    }

    public class CreateRoomRequestDto
    {
    }

    public class CreateRoomResponseDto
    {
    }

    public class JoinRoomRequestDto
    {
    }

    public class GetRoomListResponseDto
    {
    }
}