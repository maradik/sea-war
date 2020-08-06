using System;

namespace Integration.Dtos.v2
{
    public class JoinRoomRequestDto
    {
        public Guid RoomId { get; set; }
        public string PlayerName { get; set; }
    }
}