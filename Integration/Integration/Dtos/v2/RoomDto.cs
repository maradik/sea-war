using System;

namespace Integration.Dtos.v2
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public PlayerDto[] Players { get; set; }
        public RoomStatusDto Status { get; set; }
    }
}