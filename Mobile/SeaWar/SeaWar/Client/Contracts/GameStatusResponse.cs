using System;

namespace SeaWar.Client.Contracts
{
    public class GameStatusResponse
    {
        public GameStatus GameStatus { get; set; }
        public FinishReason? FinishReason { get; set; }
        public Map MyMap { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
    }
}