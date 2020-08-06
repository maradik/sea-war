using System;

namespace Backend.Controllers.v2.Dto
{
    public class JoinRoomRequestDto
    {
        public Guid RoomId { get; set; }
        public string PlayerName { get; set; }
    }
}