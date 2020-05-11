using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Models;

namespace Backend.Controllers
{
    public class RoomManager
    {
        private readonly RoomBuilder roomBuilder;
        private readonly PlayerBuilder playerBuilder;

        private readonly Dictionary<Guid, Room> rooms = new Dictionary<Guid, Room>();

        public RoomManager(RoomBuilder roomBuilder,
                           PlayerBuilder playerBuilder)
        {
            this.roomBuilder = roomBuilder;
            this.playerBuilder = playerBuilder;
        }

        public FireResponseDto Fire(FireRequestDto dto, Guid roomId, Guid playerId)
        {
            lock (rooms)
            {
                var room = rooms[roomId];
                var enemyMap = playerId == room.Player1.Id ? room.Player2.OwnMap : room.Player1.OwnMap;
                if (enemyMap.Cells[dto.Y, dto.X].Status == CellStatus.Empty)
                {
                    enemyMap.Cells[dto.Y, dto.X].Status = CellStatus.EmptyFired;
                    room.CurrentPlayerId = playerId == room.Player1.Id ? room.Player2.Id : room.Player1.Id;
                }
                else if (enemyMap.Cells[dto.Y, dto.X].Status == CellStatus.EngagedByShip)
                {
                    enemyMap.Cells[dto.Y, dto.X].Status = CellStatus.EngagedByShipFired;
                }

                return new FireResponseDto
                {
                    EnemyMap = enemyMap
                };
            }
        }

        public GetGameStatusResponseDto GetGameStatus(Guid roomId, Guid playerId)
        {
            lock (rooms)
            {
                var room = rooms[roomId];
                if (room.Player2 == null)
                {
                    //
                    return null;
                }

                var gameStatus = room.CurrentPlayerId == playerId ? GameStatus.YourChoice : GameStatus.PendingForFriendChoice;

                return new GetGameStatusResponseDto
                {
                    YourChoiceTimeout = gameStatus == GameStatus.YourChoice ? TimeSpan.FromMinutes(1) : TimeSpan.Zero,
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
                    var player = playerBuilder.Build(requestDto.PlayerName);
                    room.Player2 = player;
                    room.Player2.EnemyMap = room.Player1.OwnMap;
                    room.Player1.EnemyMap = room.Player2.OwnMap;
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
                    var player = playerBuilder.Build(requestDto.PlayerName);
                    var room = roomBuilder.Build(player, null);
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
    }
}