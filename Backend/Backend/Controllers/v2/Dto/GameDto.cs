using System;
using Backend.Controllers.v1.Dto;

namespace Backend.Controllers.v2.Dto
{
    public class GameDto
    {
        public GameStatus Status { get; set; }
        public FinishReason? FinishReason { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
        public MapDto MyMap { get; set; }
    }
}