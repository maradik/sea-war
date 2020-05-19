using System;

namespace SeaWar.Client.Contracts
{
    public class CreateRoomParameters
    {
        public string PlayerName { get; set; }
        public Guid? PlayerId { get; set; }
    }
}