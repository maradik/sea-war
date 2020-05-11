using System;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("room")]
    public class RoomController : ControllerBase
    {
        private readonly GameManager gameManager;

        public RoomController(GameManager gameManager) =>
            this.gameManager = gameManager;

        [HttpPost]
        [Route("Create")]
        public CreateRoomResponseDto Create([FromBody] CreateRoomRequestDto requestDto) =>
            gameManager.Create(requestDto);

        [HttpGet]
        [Route("{roomId}/Game/GetStatus")]
        public GetGameStatusResponseDto GetGameStatus([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            gameManager.GetGameStatus(roomId, playerId);

        [HttpPost]
        [Route("{roomId}/Game/Fire")]
        public FireResponseDto Fire([FromBody] FireRequestDto dto, [FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            gameManager.Fire(dto, roomId, playerId);

        [HttpGet]
        [Route("{roomId}/getStatus")]
        public GetRoomStatusResponseDto GetStatus([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            gameManager.GetStatus(roomId, playerId);
    }
}