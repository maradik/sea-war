using System;

namespace Integration.Dtos.v2
{
    public class GameDto
    {
        public GameStatusDto Status { get; set; }
        public FinishReasonDto? FinishReason { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
        public MapDto MyMap { get; set; }
    }
}