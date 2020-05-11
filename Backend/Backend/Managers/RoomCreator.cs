using System;
using Backend.Controllers.Dto;
using Backend.Models;

namespace Backend.Managers
{
    public class RoomCreator
    {
        private readonly PlayerBuilder playerBuilder;

        public RoomCreator(PlayerBuilder playerBuilder) =>
            this.playerBuilder = playerBuilder;

        public Room CreateRoom(CreateRoomRequestDto dto) =>
            new Room(playerBuilder)
            {
                Id = Guid.NewGuid(),
                Status = RoomStatus.EmptyRoom
            };
    }
}