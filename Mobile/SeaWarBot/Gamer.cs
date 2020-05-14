﻿using System;
using System.Threading.Tasks;
using SeaWar;
using SeaWar.Client;
using SeaWar.Client.Contracts;

namespace SeaWarBot
{
    public class Gamer
    {
        private readonly Random random = new Random();
        private readonly ILogger logger = new Logger(nameof(Gamer));
        private readonly Client client = new Client(Settings.ServerUri, Settings.Timeout, new DummyLogger());
        
        public async Task PlayAsync()
        {
            var room = await CreateRoom();
            var opponentMap = EnemyMap.Empty;
            
            while (true)
            {
                var getGameStatusParameters = new GetGameStatusParameters {PlayerId = room.PlayerId, RoomId = room.RoomId};
                var gameStatus = await client.GetGameStatusAsync(getGameStatusParameters);

                switch (gameStatus.GameStatus)
                {
                    case GameStatus.YourChoice:
                        await FireAsync(opponentMap, room);
                        break;
                    case GameStatus.PendingForFriendChoice:
                        await Task.Delay(1000);
                        break;
                    case GameStatus.Finish:
                        logger.Info($"Игра окончена, причина {gameStatus.FinishReason}");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async Task FireAsync(EnemyMap opponentMap, RoomResponse room)
        {
            while (true)
            {
                var (x, y) = (random.Next(10), random.Next(10));
                if (opponentMap.Cells[x, y].Status == EnemyCellStatus.Unknown)
                {
                    var fireParameters = new FireParameters {RoomId = room.RoomId, PlayerId = room.PlayerId, FieredCell = new CellPosition(x, y)};
                    var fireResult = await client.FireAsync(fireParameters);
                    logger.Info($"Выстрел по координате {(x,y)} с результатом {fireResult.EnemyMap.Cells[x,y].Status}");
                    return;
                }
            }
        }

        private async Task<RoomResponse> CreateRoom()
        {
            var createRoomParameters = new CreateRoomParameters {PlayerName = Guid.NewGuid().ToString()};
            var room = await client.CreateRoomAsync(createRoomParameters);
            logger.Info($"Комната создана RoomId={room.RoomId}, Status={room.RoomStatus}, PlayerName={createRoomParameters.PlayerName}");

            while (room.RoomStatus != CreateRoomStatus.Ready)
            {
                var getRoomStatusParameters = new GetRoomStatusParameters {PlayerId = room.PlayerId, RoomId = room.RoomId};
                room = await client.GetRoomStatusAsync(getRoomStatusParameters);
                await Task.Delay(1000);
            }

            logger.Info($"Комната готова к игре RoomId={room.RoomId}, Status={room.RoomStatus}, AnotherPlayerName={room.AnotherPlayerName}");
            return room;
        }
    }
}