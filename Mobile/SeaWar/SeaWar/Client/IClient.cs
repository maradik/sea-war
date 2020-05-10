using System;
using System.Threading.Tasks;
using SeaWar.Client.Contracts;

namespace SeaWar.Client
{
    public interface IClient
    {
        Task<RoomResponse> CreateRoomAsync(string playerName);
        Task<RoomResponse> GetRoomStatusAsync(Guid roomId, Guid playerId);
        Task<GameStatusResponse> GetGameStatusAsync(Guid roomId, Guid playerId);
        Task<GameFireResponse> FireAsync(Guid roomId, Guid playerId, CellPosition firedCell);
    }
}