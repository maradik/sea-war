using System;

namespace Backend.Models
{
    public class GetRoomStatusResult
    {
        [Obsolete]
        public Guid PlayerId { get; set; }
        public Guid RoomId { get; set; }
        public RoomStatus RoomStatus { get; set; }
        public string AnotherPlayerName { get; set; }
    }
}