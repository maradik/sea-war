using System;

namespace SeaWar.Client.Contracts
{
    public class FireParameters
    {
        public Guid RoomId { get; set; }
        public Guid PlayerId { get; set; }
        public CellPosition FieredCell { get; set; }
    }
}