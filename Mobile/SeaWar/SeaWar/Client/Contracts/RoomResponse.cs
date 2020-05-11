using System;

namespace SeaWar.Client.Contracts
{
    public class RoomResponse
    {
        public Guid RoomId { get; set; }
        public CreateRoomStatus RoomStatus { get; set;}
        public string AnotherPlayerName { get; set;}
        public Guid PlayerId { get; set;}
    }
}