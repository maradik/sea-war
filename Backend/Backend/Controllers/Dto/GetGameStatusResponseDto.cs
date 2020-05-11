using System;
using Backend.Models;

namespace Backend.Controllers.Dto
{
    public class GetGameStatusResponseDto
    {
        public GameStatus GameStatus { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
        public FinishReason? FinishReason { get; set; }
        public MapDto MyMap { get; set; }
    }
}