using System;

namespace Backend.Controllers.v2.Dto
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public PlayerDto[] Players { get; set; }
        public RoomStatusDto Status { get; set; }
    }
}