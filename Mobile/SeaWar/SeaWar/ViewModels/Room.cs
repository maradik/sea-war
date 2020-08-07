using System;

namespace SeaWar.ViewModels
{
    public class Room
    {
        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public string Title => $"Играть с {PlayerName}";
    }
}