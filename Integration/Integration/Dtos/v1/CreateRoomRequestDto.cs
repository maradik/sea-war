using System;

namespace Integration.Dtos.v1
{
    public class CreateRoomRequestDto
    {
        public string PlayerName { get; set; }
        public Guid? PlayerId { get; set; }
    }
}