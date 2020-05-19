using System;

namespace Backend.Controllers.Dto
{
    public class CreateRoomRequestDto
    {
        public string PlayerName { get; set; }
        public Guid? PlayerId { get; set; }
    }
}