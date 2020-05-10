using System;

namespace SeaWar.Client.Contracts
{
    public class RoomResponse
    {
        public Guid RoomId { get; }
        public CreateRoomStatus Status { get; }
        public string AnotherPlayerName { get; }
        public Guid PlayerId { get; }
    }
}