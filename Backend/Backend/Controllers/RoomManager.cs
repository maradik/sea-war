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
                var enemyMap = room.DoMove(playerId, dto.X, dto.Y);

                return new FireResponseDto
                {
                    EnemyMap = enemyMap.ToMapForEnemyDto()
                };
            }
        }

        public GetGameStatusResponseDto GetGameStatus(Guid roomId, Guid playerId)
        {
            lock (rooms)
            {
                var room = rooms[roomId];

                var gameStatus = room.GameStatus;

                return new GetGameStatusResponseDto
                {
                    YourChoiceTimeout = gameStatus == GameStatus.YourChoice ? TimeSpan.FromMinutes(1) : TimeSpan.Zero,
                    MyMap = room.Player1.Id == playerId ? room.Player1.OwnMap.ToMapDto() : room.Player2.OwnMap.ToMapDto(),
                    GameStatus = gameStatus,
                    FinishReason = gameStatus == GameStatus.Finish
                        ? room.CurrentPlayerId == playerId
                            ? FinishReason.Winner
                            : FinishReason.Lost
                        : (FinishReason?) null
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