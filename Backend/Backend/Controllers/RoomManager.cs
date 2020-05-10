using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;

namespace Backend.Controllers
{
    public class GetRoomStatusResponseDto
    {
        public Guid RoomId { get; set; }
        public RoomStatus RoomStatus { get; set; }
        public string AnotherPlayerName { get; set; }
        public Guid PlayerId { get; set; }
    }

    public class RoomManager
    {
        private readonly Dictionary<Guid, Room> rooms = new Dictionary<Guid, Room>();

        public GetRoomStatusResponseDto GetStatus(Guid roomId, Guid playerId)
        {
            lock (rooms)
            {
                var room = rooms[roomId];
                return new GetRoomStatusResponseDto
                {
                    PlayerId = room.Player1.Id,
                    RoomId = room.Id,
                    RoomStatus = room.Status,
                    AnotherPlayerName = room.Player2?.Name
                };
            }
        }

        public CreateRoomResponseDto Create(CreateRoomRequestDto requestDto)
        {
            lock (rooms)
            {
                if (rooms.Any(x => x.Value.Status == RoomStatus.NotReady))
                {
                    var room = rooms.First(x => x.Value.Status == RoomStatus.NotReady).Value;
                    var player = CreatePlayer(requestDto);
                    room.Player2 = player;
                    room.Status = RoomStatus.Ready;

                    return new CreateRoomResponseDto
                    {
                        PlayerId = player.Id,
                        RoomId = room.Id,
                        RoomStatus = RoomStatus.Ready,
                        AnotherPlayerName = room.Player1.Name
                    };
                }
                else
                {
                    var player = CreatePlayer(requestDto);
                    var room = CreateRoom(player);
                    rooms[room.Id] = room;

                    return new CreateRoomResponseDto
                    {
                        PlayerId = player.Id,
                        RoomId = room.Id,
                        RoomStatus = RoomStatus.NotReady,
                        AnotherPlayerName = null
                    };
                }
            }
        }

        private static Map CreateMap()
        {
            var cells = new Cell[10, 10];
            for (var i = 0; i < 10; ++i)
            {
                for (var j = 0; j < 10; ++j)
                {
                    cells[i, j] = new Cell
                    {
                        Status = Cell.CellStatus.Empty
                    };
                }
            }

            return new Map
            {
                Cells = cells
            };
        }

        private static Player CreatePlayer(CreateRoomRequestDto requestDto) =>
            new Player
            {
                Id = Guid.NewGuid(),
                Name = requestDto.PlayerName,
                OwnMap = CreateMap(),
                EnemyMap = CreateMap()
            };

        private static Room CreateRoom(Player player) =>
            new Room
            {
                Id = Guid.NewGuid(),
                Status = RoomStatus.NotReady,
                Player1 = player,
                Player2 = null
            };
    }


}