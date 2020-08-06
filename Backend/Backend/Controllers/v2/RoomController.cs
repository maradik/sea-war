using System;
using Backend.Controllers.v2.Dto;
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

        [HttpGet("opened")]
        public RoomListResponseDto GetOpenedRooms([FromQuery] Guid playerId) =>
            roomManager.GetOpenedRooms().ToDto();

        [HttpPost]
        public CreateRoomResponseDto Create([FromBody] CreateRoomRequestDto requestDto, [FromQuery] Guid playerId) =>
            roomManager.CreateRoom(playerId, requestDto.PlayerName).ToDto();

        [HttpPost]
        [Route("{roomId}/join")]
        public JoinRoomResponseDto Join([FromBody] JoinRoomRequestDto requestDto, [FromQuery] Guid playerId) =>
            roomManager.Join(requestDto.RoomId, playerId, requestDto.PlayerName).ToDto();

        [HttpGet]
        [Route("{roomId}")]
        public RoomDto Get([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            roomManager.GetRoom(roomId).ToDto();

        [HttpGet]
        [Route("{roomId}/game")]
        public GameDto GetGame([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            roomManager.GetGame(roomId, playerId).ToDto();

        [HttpPost]
        [Route("{roomId}/game/fire")]
        public FireResponseDto Fire([FromBody] FireRequestDto dto, [FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            roomManager.Fire(dto.X, dto.Y, roomId, playerId).ToDto();
    }
}