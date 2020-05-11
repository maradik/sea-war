using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Backend.Models;

namespace Backend.Controllers
{
    public interface IRoom
    {
        public Guid Id { get; set; }
        public RoomStatus Status { get; set; }
    }

    public class RoomCreator
    {
        public IRoom CreateRoom(CreateRoomRequestDto dto)
        {
            return new Room();
        }
    }

    public class RoomManager
    {
        private readonly PlayerBuilder playerBuilder;
        private readonly RoomBuilder roomBuilder;
        private readonly RoomCreator roomCreator;

        private readonly Dictionary<Guid, IRoom> rooms = new Dictionary<Guid, IRoom>();

        public RoomManager(PlayerBuilder playerBuilder, RoomBuilder roomBuilder, RoomCreator roomCreator)
        {
            this.playerBuilder = playerBuilder;
            this.roomBuilder = roomBuilder;
            this.roomCreator = roomCreator;
        }

        public Room GetRoom(Guid roomId)
        {
            lock (rooms)
            {
                return (Room)rooms[roomId];
            }
        }

        public CreateRoomResponseDto CreateOrEnterRoom(CreateRoomRequestDto requestDto)
        {
            lock (rooms)
            {
                var availableRoom = (Room)(rooms.FirstOrDefault(x => x.Value.Status == RoomStatus.NotReady).Value);
                if (availableRoom != null)
                {
                    return availableRoom.Enter(requestDto);
                }

                var room = (Room)roomCreator.CreateRoom(requestDto);
                rooms[room.Id] = room;
                return room.Enter(requestDto);
            }
        }
    }
}