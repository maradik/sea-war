using System;

namespace Integration.Dtos.v1
{
    public class GetRoomStatusResponseDto
    {
        public Guid RoomId { get; set; }
        public RoomStatusDto RoomStatus { get; set; }
        public string AnotherPlayerName { get; set; }

        [Obsolete]
        public Guid PlayerId { get; set; }
    }
}