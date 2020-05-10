using System;

namespace Backend.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public RoomStatus Status { get; set; }
        public Guid CurrentPlayerId { get; set; }
    }
}