namespace Integration.Dtos.v2
{
    public class CreateRoomRequestDto
    {
        public string PlayerName { get; set; }
        public RoomTypeDto RoomType { get; set; }
    }
}