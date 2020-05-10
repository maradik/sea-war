using System;

namespace SeaWar.Client.Contracts
{
    public class GetRoomStatusParameters
    {
        public Guid RoomId { get; set; }
        public Guid PlayerId { get; set; }
    }
}