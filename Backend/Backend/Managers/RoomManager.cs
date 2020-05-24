using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Backend.Controllers.v1.Dto;
using Backend.Models;

namespace Backend.Managers
{
    public class RoomManager
    {
        private readonly RoomCreator roomCreator;
        private readonly ConcurrentDictionary<Guid, Room> rooms = new ConcurrentDictionary<Guid, Room>();

        public RoomManager(RoomCreator roomCreator) =>
            this.roomCreator = roomCreator;

        public GetRoomStatusResult GetStatus(Guid roomId, Guid playerId)
        {
            var room = rooms[roomId];
            room.Touch();
            return new GetRoomStatusResult
            {
                PlayerId = playerId,
                RoomId = room.Id,
                RoomStatus = room.Status,
                AnotherPlayerName = room.GetEnemyPlayerFor(playerId)?.Name
            };
        }

        public FireResponseDto Fire(FireRequestDto dto, Guid roomId, Guid playerId) =>
            rooms[roomId].Fire(dto, playerId);

        public GetGameStatusResponseDto GetGameStatus(Guid roomId, Guid playerId) =>
            rooms[roomId].GetGameStatus(playerId);

        [Obsolete]
        public EnterOrCreateRoomResult EnterOrCreateRoom(Guid playerId, string playerName)
        {
            lock (rooms)
            {
                UpdateRoomStatuses(playerId);
                var room = GetOrCreateRoom();
                var player = room.Enter(playerId, playerName);

                return new EnterOrCreateRoomResult
                {
                    PlayerId = player.Id,
                    RoomId = room.Id,
                    RoomStatus = room.Status,
                    AnotherPlayerName = room.GetEnemyPlayerFor(player.Id)?.Name
                };
            }
        }

        private Room GetOrCreateRoom()
        {
            var room = rooms.FirstOrDefault(x => x.Value.Status == RoomStatus.NotReady).Value ?? roomCreator.CreateRoom();
            rooms[room.Id] = room;
            return room;
        }

        private void UpdateRoomStatuses(Guid playerId)
        {
            foreach (var room in rooms.Select(x => x.Value))
            {
                if (room.HasActiveStatus && (room.Player1?.Id == playerId || room.Player2?.Id == playerId))
                {
                    room.Status = RoomStatus.Orphaned;
                }

                //TODO удалять старые закрытые комнаты, чтобы не случился OOM
            }
        }
    }
}