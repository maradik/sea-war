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

        public async Task<RoomResponse> CreateRoomAsync(CreateRoomParameters parameters)
        {
            var response = await httpClient.PostAsync("/Room/Create", new StringContent(parameters.PlayerName)).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<RoomResponse>(json);
        }

        public async Task<RoomResponse> GetRoomStatusAsync(GetRoomStatusParameters parameters)
        {
            var response = await httpClient.GetAsync($"/Room/{parameters.RoomId}/GetStatus?playerId={parameters.PlayerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<RoomResponse>(json);
        }

        public async Task<GameStatusResponse> GetGameStatusAsync(GetGameStatusParameters parameters)
        {
            var response = await httpClient.GetAsync($"/Room/{parameters.RoomId}/Game/GetStatus?playerId={parameters.PlayerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GameStatusResponse>(json);
        }

        public async Task<GameFireResponse> FireAsync(FireParameters parameters)
        {
            var content = new StringContent(JsonConvert.SerializeObject(parameters.FieredCell));
            var response = await httpClient.PostAsync($"/Room/{parameters.RoomId}/Game/Fire?playerId={parameters.PlayerId}", content).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<GameFireResponse>(json);
        }
    }
}