using System;
using System.Linq;
using System.Threading.Tasks;
using Integration.Dtos.v2;
using SeaWar;
using SeaWar.Client;

namespace SeaWarBot
{
    public class Gamer
    {
        private static string[] playerNames = {"Федот-стрелец", "Иван Петрович", "Михалыч", "Super killer", "Marika-38"};
        private readonly Random random = new Random();
        private readonly ILogger logger = new Logger(nameof(Gamer));
        private readonly Client client = new Client("http://127.0.0.1:8765/", Settings.Timeout, new DummyLogger());
        
        private readonly Guid playerId = Guid.NewGuid();
        private string playerName;

        public Gamer() =>
            playerName = playerNames[random.Next(playerNames.Length)];

        public async Task PlayAsync()
        {
            logger.Info($"Я в игре, мое имя {playerName}, id={playerId}");

            var room = await GetRoomAsync().ConfigureAwait(false);
            var opponentMap = CreateEmptyOpponentMapDto();
            
            while (true)
            {
                var game = await client.GetGameAsync(room.Id, playerId).ConfigureAwait(false);

                switch (game.Status)
                {
                    case GameStatusDto.YourChoice:
                        opponentMap = await FireAsync(opponentMap, room).ConfigureAwait(false);
                        break;
                    case GameStatusDto.PendingForFriendChoice:
                        await Task.Delay(1000).ConfigureAwait(false);
                        break;
                    case GameStatusDto.Finish:
                        logger.Info($"Игра окончена, причина {game.FinishReason}");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private MapForEnemyDto CreateEmptyOpponentMapDto()
        {
            var mapForEnemyDto = new MapForEnemyDto();
            for (var x = 0; x < mapForEnemyDto.Cells.GetLength(0); x++)
            {
                for (var y = 0; y < mapForEnemyDto.Cells.GetLength(1); y++)
                {
                    mapForEnemyDto.Cells[x, y] = new CellForEnemyDto();
                }
            }

            return mapForEnemyDto;
        }

        private async Task<MapForEnemyDto> FireAsync(MapForEnemyDto opponentMap, RoomDto room)
        {
            while (true)
            {
                var (x, y) = (random.Next(10), random.Next(10));
                if (opponentMap.Cells[x, y].Status == CellForEnemyDtoStatus.Unknown)
                {
                    var fireRequest = new FireRequestDto {X = x, Y = y};
                    var fireResult = await client.FireAsync(fireRequest, room.Id, playerId).ConfigureAwait(false);
                    logger.Info($"Выстрел по координате {(x,y)} с результатом {fireResult.EnemyMap.Cells[x,y].Status}");
                    return fireResult.EnemyMap;
                }
            }
        }

        private async Task<RoomDto> GetRoomAsync() =>
            await TryJoinOpenedRoomAsync().ConfigureAwait(false) ?? await CreateRoomAsync().ConfigureAwait(false);

        private async Task<RoomDto> CreateRoomAsync()
        {
            var createRoomRequest = new CreateRoomRequestDto {PlayerName = playerName};
            var createdRoom = await client.CreateRoomAsync(createRoomRequest, playerId).ConfigureAwait(false);
            
            logger.Info($"Создана комната {createdRoom.RoomId}, ожидаем соперника");

            while (true)
            {
                var room = await client.GetRoomAsync(createdRoom.RoomId, playerId).ConfigureAwait(false);
                if (room.Status == RoomStatusDto.Ready)
                {
                    logger.Info($"В комнату {room.Id} подключился соперник {room.Players.First(x => x.Id != playerId).Name}");
                    return room;
                }

                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        private async Task<RoomDto> TryJoinOpenedRoomAsync()
        {
            var roomList = await client.GetOpenedRoomsAsync(playerId).ConfigureAwait(false);
            
            logger.Info($"Найдено {roomList.Rooms.Length} открытых комнат");
            
            var openedRoom = roomList.Rooms.FirstOrDefault();
            if (openedRoom != default)
            {
                logger.Info($"Пытаюсь подключиться к комнате {openedRoom.Id}");
                
                var joinRequest = new JoinRoomRequestDto {PlayerName = playerName};
                var joinResult = await client.JoinRoomAsync(joinRequest, openedRoom.Id, playerId).ConfigureAwait(false);
                if (joinResult.Success)
                {
                    var room = await client.GetRoomAsync(openedRoom.Id, playerId).ConfigureAwait(false);
                    
                    logger.Info($"Удалось подключиться к комнате {room.Id}, имя соперника {room.Players.First(x => x.Id != playerId).Name}");
                    
                    return room;
                }
            }

            return default;
        }
    }
}