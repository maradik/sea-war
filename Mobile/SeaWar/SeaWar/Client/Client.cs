using System;
using System.Threading.Tasks;
using SeaWar.Client.Contracts;

namespace SeaWar.Client
{
    public class Client : IClient
    {
        public Task<RoomResponse> CreateRoomAsync(string playerName)
        {
            throw new NotImplementedException();
        }

        public Task<RoomResponse> GetRoomStatusAsync(Guid roomId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Task<GameStatusResponse> GetGameStatusAsync(Guid roomId, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public Task<GameFireResponse> FireAsync(Guid roomId, Guid playerId, CellPosition firedCell)
        {
            throw new NotImplementedException();
        }
    }
}