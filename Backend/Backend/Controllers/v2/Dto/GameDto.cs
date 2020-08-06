using System;

namespace Backend.Controllers.v2.Dto
{
    public class GameDto
    {
        public GameStatusDto Status { get; set; }
        public FinishReasonDto? FinishReason { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
        public MapDto MyMap { get; set; }
    }
}