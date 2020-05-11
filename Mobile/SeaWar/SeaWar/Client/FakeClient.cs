using System;
using System.Threading.Tasks;
using SeaWar.Client.Contracts;

namespace SeaWar.Client
{
    public class FakeClient : IClient
    {
        private static Random random = new Random();
        private int countTryReadyToPlay;

        public Task<RoomResponse> CreateRoomAsync(CreateRoomParameters parameters)
        {
            return Task.FromResult(new RoomResponse()
            {
                Status = CreateRoomStatus.WaitingForAnotherPlayer,
                PlayerId = Guid.NewGuid(),
                RoomId = Guid.NewGuid(),
                AnotherPlayerName = nameof(RoomResponse.AnotherPlayerName)
            });
        }

        public Task<RoomResponse> GetRoomStatusAsync(GetRoomStatusParameters parameters)
        {
            countTryReadyToPlay++;
            var status = countTryReadyToPlay > 1
                ? CreateRoomStatus.ReadyForStart
                : CreateRoomStatus.WaitingForAnotherPlayer;
            
            return Task.FromResult(new RoomResponse()
            {
                Status = status,
                PlayerId = parameters.PlayerId,
                RoomId = parameters.RoomId,
                AnotherPlayerName = nameof(RoomResponse.AnotherPlayerName)
            });
        }

        public Task<GameStatusResponse> GetGameStatusAsync(GetGameStatusParameters parameters)
        {
            return Task.FromResult(new GameStatusResponse
            {
                GameStatus = (GameStatus) random.Next(2),
                MyMap = new Map 
                {
                    Cells = new[,]
                    {
                        {new Cell{Status = Cell.CellStatus.EmptyFired}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                    }
                }
            });
        }

        public Task<GameFireResponse> FireAsync(FireParameters parameters)
        {
            return Task.FromResult(new GameFireResponse
            {
                OpponentMap = new Map
                {
                    Cells = new[,]
                    {
                        {new Cell{Status = Cell.CellStatus.EngagedByShipFired}, new Cell{Status = Cell.CellStatus.EmptyFired}, new Cell{Status = Cell.CellStatus.EmptyFired}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                        {new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
                    }
                }
            });
        }
    }
}