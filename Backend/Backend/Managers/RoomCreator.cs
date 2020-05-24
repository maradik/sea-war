using System;
using Backend.Controllers.v1.Dto;
using Backend.Models;

namespace Backend.Managers
{
    public class RoomCreator
    {
        private readonly PlayerBuilder playerBuilder;

        public RoomCreator(PlayerBuilder playerBuilder) =>
            this.playerBuilder = playerBuilder;

        public Room CreateRoom() =>
            new Room(playerBuilder)
            {
                Id = Guid.NewGuid(),
                Status = RoomStatus.EmptyRoom
            };
    }
}