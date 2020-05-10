using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    public class FireRequestDto
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<RoomController> _logger;
        private readonly RoomManager roomManager;

        public RoomController(ILogger<RoomController> logger,
                              RoomManager roomManager)
        {
            _logger = logger;
            this.roomManager = roomManager;
        }

        [HttpGet]
        [Route("room/{roomId}/Game/GetStatus")]
        public IActionResult GetGameStatus([FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            Ok(roomManager.GetGameStatus(roomId, playerId));

        [HttpPost]
        [Route("room/{roomId}/Game/Fire")]
        public IActionResult Fire([FromBody] FireRequestDto dto, [FromRoute] Guid roomId, [FromQuery] Guid playerId) =>
            Ok(roomManager.Fire(dto, roomId, playerId));

        [HttpPost]
        [Route("room/create")]
        public IActionResult Create([FromBody] CreateRoomRequestDto requestDto) =>
            Ok(roomManager.Create(requestDto));

        [HttpGet]
        [Route("room/{roomId}/getStatus")]
        public IActionResult GetStatus([FromRoute] Guid roomId, [FromQuery] Guid userId) =>
            Ok(roomManager.GetStatus(roomId, userId));
    }
}