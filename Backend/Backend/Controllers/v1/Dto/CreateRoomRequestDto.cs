using System;

namespace Backend.Controllers.v1.Dto
{
    public class CreateRoomRequestDto
    {
        public string PlayerName { get; set; }
        public Guid? PlayerId { get; set; }
    }
}