using System;
using System.Threading.Tasks;
using SeaWar.Client.Contracts;

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


        public async Task<RoomResponse> CreateRoomAsync(CreateRoomParameters parameters)
        {
            return await RetryAsync(client.CreateRoomAsync, parameters).ConfigureAwait(false);
        }

        public async Task<RoomResponse> GetRoomStatusAsync(GetRoomStatusParameters parameters)
        {
            return await RetryAsync(client.GetRoomStatusAsync, parameters).ConfigureAwait(false);
        }

        public async Task<GameStatusResponse> GetGameStatusAsync(GetGameStatusParameters parameters)
        {
            return await RetryAsync(client.GetGameStatusAsync, parameters).ConfigureAwait(false);
        }

        public async Task<GameFireResponse> FireAsync(FireParameters parameters)
        {
            return await RetryAsync(client.FireAsync, parameters).ConfigureAwait(false);
        }

        public async Task<T2> RetryAsync<T1, T2>( Func<T1, Task<T2>> func, T1 parameters)
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
                    await Task.Delay(toSleep);
                    toSleep = TimeSpan.FromMilliseconds(toSleep.TotalMilliseconds * 1.5);
                }
            }
            throw exception;
        }
    }
}