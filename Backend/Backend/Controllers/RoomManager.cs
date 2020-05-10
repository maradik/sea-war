using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;

namespace Backend.Controllers
{
    public enum GameStatus
    {
        YourChoice,
        PendingForFriendChoice,
        Finish
    }

    public enum FinishReason
    {
        ConnectionLost,
        Winner,
        Lost
    }

    public class GetGameStatusResponseDto
    {
        public GameStatus GameStatus { get; set; }
        public TimeSpan YourChoiceTimeout { get; set; }
        public FinishReason? FinishReason { get; set; }
        public Map MyMap { get; set; }
    }

    public class RoomManager
    {
        private readonly Dictionary<Guid, Room> rooms = new Dictionary<Guid, Room>();

        public GetGameStatusResponseDto GetGameStatus(Guid roomId, Guid playerId)
        {
            lock (rooms)
            {
                var room = rooms[roomId];
                if (room.Player2 == null)
                {
                    //
                }

                var gameStatus = room.CurrentPlayerId == playerId ? GameStatus.YourChoice : GameStatus.PendingForFriendChoice;

                return new GetGameStatusResponseDto
                {
                    YourChoiceTimeout = gameStatus == GameStatus.YourChoice ? TimeSpan.FromMinutes(1) : TimeSpan.MaxValue,
                    MyMap = room.Player1.Id == playerId ? room.Player1.OwnMap : room.Player2.OwnMap,
                    GameStatus = gameStatus,
                    FinishReason = null
                };
            }
        }

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
                Player2 = null,
                CurrentPlayerId = player.Id
            };
    }


}