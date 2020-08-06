using System;

namespace Backend.Controllers.v1.Dto
{
    public class CreateRoomResponseDto
    {
        public Guid RoomId { get; set; }
        public RoomStatusDto RoomStatus { get; set; }
        public string AnotherPlayerName { get; set; }

        [Obsolete]
        public Guid PlayerId { get; set; }
    }
}