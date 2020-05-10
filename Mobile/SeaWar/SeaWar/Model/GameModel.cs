using System;

namespace SeaWar.Model
{
    public class GameModel
    {
        public string PlayerName { get; set; }
        public string AnotherPlayerName { get; set; }
        public Guid PlayerId { get; set; }
        public Guid RoomId { get; set; }
    }
}