using System.Threading.Tasks;
using SeaWar.Client.Contracts;

namespace SeaWar.Client
{
    public interface IClient
    {
        Task<RoomResponse> CreateRoomAsync(CreateRoomParameters parameters);
        Task<RoomResponse> GetRoomStatusAsync(GetRoomStatusParameters parameters);
        Task<GameStatusResponse> GetGameStatusAsync(GetGameStatusParameters parameters);
        Task<GameFireResponse> FireAsync(FireParameters parameters);
    }
}