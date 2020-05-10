using System;

namespace SeaWar.DomainModels
{
    public class GameModel
    {
        public string PlayerName { get; set; }
        public string AnotherPlayerName { get; set; }
        public Guid PlayerId { get; set; }
        public Guid RoomId { get; set; }
    }
}