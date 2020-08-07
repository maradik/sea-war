using System;
using System.Threading.Tasks;
using Integration.Dtos.v2;

namespace SeaWar.Client
{
    public class RetryableClient : IClient
    {
        private readonly IClient client;
        private readonly int retryCount;

        public RetryableClient(IClient client, int retryCount)
        {
            this.client = client;
            this.retryCount = retryCount;
        }

        public Task<CreateRoomResponseDto> CreateRoomAsync(CreateRoomRequestDto parameters, Guid playerId) =>
            RetryAsync(x => client.CreateRoomAsync(x.parameters, x.playerId), (parameters, playerId));

        public Task<JoinRoomResponseDto> JoinRoomAsync(JoinRoomRequestDto parameters, Guid roomId, Guid playerId) =>
            RetryAsync(x => client.JoinRoomAsync(x.parameters, x.roomId, x.playerId), (parameters, roomId, playerId));

        public Task<RoomListResponseDto> GetOpenedRoomsAsync(Guid playerId) =>
            RetryAsync(x => client.GetOpenedRoomsAsync(x), playerId);

        public Task<RoomDto> GetRoomAsync(Guid roomId, Guid playerId) =>
            RetryAsync(x => client.GetRoomAsync(x.roomId, x.playerId), (roomId, playerId));

        public Task<GameDto> GetGameAsync(Guid roomId, Guid playerId) =>
            RetryAsync(x => client.GetGameAsync(x.roomId, x.playerId), (roomId, playerId));

        public Task<FireResponseDto> FireAsync(FireRequestDto parameters, Guid roomId, Guid playerId) =>
            RetryAsync(x => client.FireAsync(x.parameters, x.roomId, x.playerId), (parameters, roomId, playerId));

        private async Task<T2> RetryAsync<T1, T2>(Func<T1, Task<T2>> func, T1 parameters)
        {
            var retries = 0;
            var toSleep = TimeSpan.FromMilliseconds(1000);
            Exception exception = null;
            while (retries < retryCount)
            {
                try
                {
                    return await func(parameters).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    exception = e;
                }

                retries++;
                if (retries < retryCount)
                {
                    await Task.Delay(toSleep).ConfigureAwait(false);
                    toSleep = TimeSpan.FromMilliseconds(toSleep.TotalMilliseconds * 1.5);
                }
            }

            throw exception;
        }
    }
}