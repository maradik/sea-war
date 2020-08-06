using System;
using Backend.Controllers.v1.Dto;

namespace Backend.Models
{
    public class Game
    {
        public GameStatus GameStatus { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
        public FinishReason? FinishReason { get; set; }
        public Map MyMap { get; set; }
    }
}