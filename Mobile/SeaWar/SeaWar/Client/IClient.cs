using System;
using System.Threading.Tasks;
using Integration.Dtos.v2;

namespace SeaWar.Client
{
    public interface IClient
    {
        Task<CreateRoomResponseDto> CreateRoomAsync(CreateRoomRequestDto parameters, Guid playerId);
        Task<JoinRoomResponseDto> JoinRoomAsync(JoinRoomRequestDto parameters, Guid roomId, Guid playerId);
        Task<RoomListResponseDto> GetOpenedRoomsAsync(Guid playerId);
        Task<RoomDto> GetRoomAsync(Guid roomId, Guid playerId);
        Task<GameDto> GetGameAsync(Guid roomId, Guid playerId);
        Task<FireResponseDto> FireAsync(FireRequestDto parameters, Guid roomId, Guid playerId);
    }
}