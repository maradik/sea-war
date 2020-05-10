using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
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

        [HttpPost]
        [Route("room/create")]
        public IActionResult Create([FromBody] CreateRoomRequestDto requestDto) =>
            Ok(roomManager.Create(requestDto));

        [HttpGet]
        [Route("room/{roomId}/getStatus")]
        public IActionResult GetStatus([FromRoute] Guid roomId, [FromQuery] Guid userId)
        {
            // if room not exists then return 404
            return Ok(roomManager.GetStatus(roomId, userId));
        }
    }
}