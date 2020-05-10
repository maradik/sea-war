using System;

namespace SeaWar.Client.Contracts
{
    public class GetGameStatusParameters
    {
        public Guid RoomId { get; set; }
        public Guid PlayerId { get; set; }
    }
}