using System;
using Backend.Models;

namespace Backend.Controllers.Dto
{
    public class CreateRoomResponseDto
    {
        public Guid RoomId { get; set; }
        public RoomStatus RoomStatus { get; set; }
        public string AnotherPlayerName { get; set; }
        
        [Obsolete]
        public Guid PlayerId { get; set; }
    }
}