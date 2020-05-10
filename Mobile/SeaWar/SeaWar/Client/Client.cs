using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SeaWar.Client.Contracts;

namespace SeaWar.Client
{
    public class Client : IClient
    {
        private readonly HttpClient httpClient;
        public Client(string baseUri)
        {
            httpClient = new HttpClient {BaseAddress = new Uri(baseUri)};
        }

        public async Task<RoomResponse> CreateRoomAsync(string playerName)
        {
            var response = await httpClient.PostAsync("/Room/Create", new StringContent(playerName)).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<RoomResponse>(json);
        }

        public async Task<RoomResponse> GetRoomStatusAsync(Guid roomId, Guid playerId)
        {
            var response = await httpClient.GetAsync($"/Room/{roomId}/GetStatus?playerId={playerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<RoomResponse>(json);
        }

        public async Task<GameStatusResponse> GetGameStatusAsync(Guid roomId, Guid playerId)
        {
            var response = await httpClient.GetAsync($"/Room/{roomId}/Game/GetStatus?playerId={playerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GameStatusResponse>(json);
        }

        public async Task<GameFireResponse> FireAsync(Guid roomId, Guid playerId, CellPosition firedCell)
        {
            var content = new StringContent(JsonConvert.SerializeObject(firedCell));
            var response = await httpClient.PostAsync($"/Room/{roomId}/Game/Fire?playerId={playerId}", content).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GameFireResponse>(json);
        }
    }
}