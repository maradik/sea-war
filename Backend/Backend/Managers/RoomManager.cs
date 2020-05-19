using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Controllers.Dto;
using Backend.Models;

namespace Backend.Managers
{
    public class RoomManager
    {
        private readonly RoomCreator roomCreator;
        private readonly Dictionary<Guid, Room> rooms = new Dictionary<Guid, Room>();

        public RoomManager(RoomCreator roomCreator) =>
            this.roomCreator = roomCreator;

        public Room GetRoom(Guid roomId)
        {
            var room = rooms[roomId];
            lock (room)
            {
                return room;
            }
        }

        public CreateRoomResponseDto CreateOrEnterRoom(CreateRoomRequestDto requestDto)
        {
            lock (rooms)
            {
                UpdateRoomStatuses(requestDto);

                var availableRoom = rooms.FirstOrDefault(x => x.Value.Status == RoomStatus.NotReady).Value;
                if (availableRoom != null)
                {
                    return availableRoom.Enter(requestDto);
                }

                var room = roomCreator.CreateRoom(requestDto);
                rooms[room.Id] = room;
                return room.Enter(requestDto);
            }
        }

        private void UpdateRoomStatuses(CreateRoomRequestDto requestDto)
        {
            foreach (var room in rooms.Select(x => x.Value))
            {
                room.UpdateStatusIfNeeded();

                if (room.HasActiveStatus &&
                    (room.Player1 != null && room.Player1.Id == requestDto.PlayerId ||
                    room.Player2 != null && room.Player2.Id == requestDto.PlayerId))
                {
                    room.Status = RoomStatus.Orphaned;
                }
                //TODO удалять старые закрытые комнаты, чтобы не случился OOM
            }
        }
    }
}