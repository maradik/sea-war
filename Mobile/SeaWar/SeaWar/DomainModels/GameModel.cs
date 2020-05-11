using System;

namespace SeaWar.DomainModels
{
    public class GameModel
    {
        public static int MapHorizontalSize = 10;
        public static int MapVerticalSize = 10;
        
        public string PlayerName { get; set; }
        public string AnotherPlayerName { get; set; }
        public Guid PlayerId { get; set; }
        public Guid RoomId { get; set; }
    }
}