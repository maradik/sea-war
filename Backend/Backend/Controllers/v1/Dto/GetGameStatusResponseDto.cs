using System;

namespace Backend.Controllers.v1.Dto
{
    public class GetGameStatusResponseDto
    {
        public GameStatusDto GameStatus { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
        public FinishReasonDto? FinishReason { get; set; }
        public MapDto MyMap { get; set; }
    }
}