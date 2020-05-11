using System;
using System.Threading.Tasks;
using SeaWar.Client.Contracts;

namespace SeaWar.Client
{
    public class FakeClient : IClient
    {
        private int countTryReadyToPlay;

        public Task<RoomResponse> CreateRoomAsync(CreateRoomParameters parameters)
        {
            return Task.FromResult(new RoomResponse()
            {
                Status = CreateRoomStatus.WaitingForAnotherPlayer,
                PlayerId = Guid.NewGuid(),
                RoomId = Guid.NewGuid(),
                AnotherPlayerName = nameof(RoomResponse.AnotherPlayerName)
            });
        }

        public Task<RoomResponse> GetRoomStatusAsync(GetRoomStatusParameters parameters)
        {
            countTryReadyToPlay++;
            var status = countTryReadyToPlay > 1
                ? CreateRoomStatus.ReadyForStart
                : CreateRoomStatus.WaitingForAnotherPlayer;
            
            return Task.FromResult(new RoomResponse()
            {
                Status = status,
                PlayerId = parameters.PlayerId,
                RoomId = parameters.RoomId,
                AnotherPlayerName = nameof(RoomResponse.AnotherPlayerName)
            });
        }

        public Task<GameStatusResponse> GetGameStatusAsync(GetGameStatusParameters parameters)
        {
            return Task.FromResult(new GameStatusResponse()
            {
                MyMap = { }
            });
        }

        public Task<GameFireResponse> FireAsync(FireParameters parameters)
        {
            return Task.FromResult(new GameFireResponse()
            {
                OpponentMap = { }
            });
        }
    }
}