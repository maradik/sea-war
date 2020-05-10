using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet]
        [Route("room/{roomId}/getStatus")]
        public ActionResult GetStatus()
        {
            var status = roomManager.GetStatus();
            return Ok(status);
        }

        [HttpPost]
        [Route("")]
        public IActionResult SetShip(Ship ship, int x, int y)
        {
            roomManager.SetShip(ship, x, y);
        }
    }
}