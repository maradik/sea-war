using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Integration.Dtos.v2;
using Newtonsoft.Json;

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

        public async Task<CreateRoomResponseDto> CreateRoomAsync(CreateRoomRequestDto parameters, Guid playerId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"/v2/rooms?playerId={playerId}", content).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            logger.Info($"{nameof(CreateRoomAsync)}:Response:{json}");

            return JsonConvert.DeserializeObject<CreateRoomResponseDto>(json);
        }

        public async Task<JoinRoomResponseDto> JoinRoomAsync(JoinRoomRequestDto parameters, Guid roomId, Guid playerId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"/v2/rooms/{roomId}/join?playerId={playerId}", content).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            logger.Info($"{nameof(JoinRoomAsync)}:Response:{json}");

            return JsonConvert.DeserializeObject<JoinRoomResponseDto>(json);
        }

        public async Task<RoomListResponseDto> GetOpenedRoomsAsync(Guid playerId)
        {
            var response = await httpClient.GetAsync($"/v2/rooms/opened?playerId={playerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            logger.Info($"{nameof(GetOpenedRoomsAsync)}:Response:{json}");

            return JsonConvert.DeserializeObject<RoomListResponseDto>(json);
        }

        public async Task<RoomDto> GetRoomAsync(Guid roomId, Guid playerId)
        {
            var response = await httpClient.GetAsync($"/v2/rooms/{roomId}?playerId={playerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            logger.Info($"{nameof(GetRoomAsync)}:Response:{json}");

            return JsonConvert.DeserializeObject<RoomDto>(json);
        }

        public async Task<GameDto> GetGameAsync(Guid roomId, Guid playerId)
        {
            var response = await httpClient.GetAsync($"/v2/rooms/{roomId}/game?playerId={playerId}").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            logger.Info($"{nameof(GetGameAsync)}:Response:{json}");

            return JsonConvert.DeserializeObject<GameDto>(json);
        }

        public async Task<FireResponseDto> FireAsync(FireRequestDto parameters, Guid roomId, Guid playerId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"/v2/rooms/{roomId}/game/fire?playerId={playerId}", content).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            logger.Info($"{nameof(FireAsync)}:Response:{json}");

            return JsonConvert.DeserializeObject<FireResponseDto>(json);
        }
    }
}