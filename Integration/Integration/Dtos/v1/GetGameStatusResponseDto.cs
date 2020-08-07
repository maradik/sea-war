using System;

namespace Integration.Dtos.v1
{
    public class GetGameStatusResponseDto
    {
        public GameStatusDto GameStatus { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
        public FinishReasonDto? FinishReason { get; set; }
        public MapDto MyMap { get; set; }
    }
}