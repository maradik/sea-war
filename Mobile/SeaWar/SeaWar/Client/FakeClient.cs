using System;
using System.Threading.Tasks;
using SeaWar.Client.Contracts;
using SeaWar.DomainModels;
using Xamarin.Forms.Xaml;

namespace SeaWar.Client
{
    public class FakeClient : IClient
    {
        private static Random random = new Random();
        private int countTryReadyToPlay;

        public Task<RoomResponse> CreateRoomAsync(CreateRoomParameters parameters)
        {
            return Task.FromResult(new RoomResponse()
            {
                RoomStatus = CreateRoomStatus.NotReady,
                PlayerId = Guid.NewGuid(),
                RoomId = Guid.NewGuid(),
                AnotherPlayerName = nameof(RoomResponse.AnotherPlayerName)
            });
        }

        public Task<RoomResponse> GetRoomStatusAsync(GetRoomStatusParameters parameters)
        {
            countTryReadyToPlay++;
            var status = countTryReadyToPlay > 1
                ? CreateRoomStatus.Ready
                : CreateRoomStatus.NotReady;

            return Task.FromResult(new RoomResponse()
            {
                RoomStatus = status,
                PlayerId = parameters.PlayerId,
                RoomId = parameters.RoomId,
                AnotherPlayerName = nameof(RoomResponse.AnotherPlayerName)
            });
        }

        public async Task<GameStatusResponse> GetGameStatusAsync(GetGameStatusParameters parameters)
        {
            await Task.Delay(2000);
            var map = Map.Empty;
            map.Cells[random.Next(GameModel.MapHorizontalSize), random.Next(GameModel.MapHorizontalSize)].Status = Cell.CellStatus.EngagedByShipFired;
            return new GameStatusResponse
            {
                GameStatus = (GameStatus) random.Next(2),
                MyMap = map
            };
        }

        public Task<GameFireResponse> FireAsync(FireParameters parameters)
        {
            var enemyMap = EnemyMap.Empty;
            enemyMap.Cells[parameters.FieredCell.X, parameters.FieredCell.Y].Status = EnemyCellStatus.Damaged;
            return Task.FromResult(new GameFireResponse
            {
                EnemyMap = enemyMap
            });
        }
    }
}