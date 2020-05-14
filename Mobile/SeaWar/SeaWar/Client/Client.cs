using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SeaWar.Client.Contracts;

namespace SeaWar.Client
{
    public class Client : IClient
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        public Client(string baseUri, TimeSpan timeout, ILogger logger)
        {
            this.logger = logger.WithContext("HttpClient");
            httpClient = new HttpClient {BaseAddress = new Uri(baseUri)};
            httpClient.Timeout = timeout;
        }

        public async Task<RoomResponse> CreateRoomAsync(CreateRoomParameters parameters)
        {
            var content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/Room/Create", content).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            logger.Info($"{nameof(CreateRoomAsync)}:Response:{json}");
            
            return JsonConvert.DeserializeObject<RoomResponse>(json);
        }

        public async Task<RoomResponse> GetRoomStatusAsync(GetRoomStatusParameters parameters)
        {
            var response = await httpClient.GetAsync($"/Room/{parameters.RoomId}/GetStatus?playerId={parameters.PlayerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            logger.Info($"{nameof(GetRoomStatusAsync)}:Response:{json}");
            
            return JsonConvert.DeserializeObject<RoomResponse>(json);
        }

        public async Task<GameStatusResponse> GetGameStatusAsync(GetGameStatusParameters parameters)
        {
            var response = await httpClient.GetAsync($"/Room/{parameters.RoomId}/Game/GetStatus?playerId={parameters.PlayerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            logger.Info($"{nameof(GetGameStatusAsync)}:Response:{json}");
            
            return JsonConvert.DeserializeObject<GameStatusResponse>(json);
        }

        public async Task<GameFireResponse> FireAsync(FireParameters parameters)
        {
            var content = new StringContent(JsonConvert.SerializeObject(parameters.FieredCell), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"/Room/{parameters.RoomId}/Game/Fire?playerId={parameters.PlayerId}", content).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            logger.Info($"{nameof(FireAsync)}:Response:{json}");
            
            return JsonConvert.DeserializeObject<GameFireResponse>(json);
        }
    }
}