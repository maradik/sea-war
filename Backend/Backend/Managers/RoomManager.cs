using System;
using System.Collections.Concurrent;
using System.Linq;
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

        public FireResponse Fire(int x, int y, Guid roomId, Guid playerId) =>
            rooms[roomId].Fire(x, y, playerId);

        public Game GetGame(Guid roomId, Guid playerId) =>
            rooms[roomId].GetGameFor(playerId);

        public CreateRoomResult CreateRoom(Guid playerId, string playerName)
        {
            var room = roomCreator.CreateRoom();
            room.Enter(playerId, playerName);
            rooms.TryAdd(room.Id, room);

            return new CreateRoomResult
            {
                RoomId = room.Id
            };
        }

        public JoinRoomResult Join(Guid roomId, Guid playerId, string playerName)
        {
            lock (rooms)
            {
                UpdateRoomStatuses(playerId);
                if (rooms.TryGetValue(roomId, out var room))
                {
                    try
                    {
                        room.Enter(playerId, playerName);

                        return new JoinRoomResult
                        {
                            Success = true
                        };
                    }
                    catch (Exception e) when (e is ArgumentException || e is InvalidOperationException)
                    {
                    }
                }
            }

            return new JoinRoomResult
            {
                Success = false
            };
        }

        public Room[] GetOpenedRooms()
        {
            return rooms.Values.Where(x => x.IsOpened).OrderByDescending(x => x.LastActivityTicks).ToArray();
        }

        public Room GetRoom(Guid roomId)
        {
            rooms.TryGetValue(roomId, out var room);
            return room;
        }

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
                if (room.IsActive && (room.Player1?.Id == playerId || room.Player2?.Id == playerId))
                {
                    room.Status = RoomStatus.Orphaned;
                }

                //TODO удалять старые закрытые комнаты, чтобы не случился OOM
            }
        }
    }
}