﻿using System;
using System.Threading.Tasks;
using SeaWar.Client.Contracts;

namespace SeaWar.Client
{
    public class FakeClient : IClient
    {
        private int countTryReadyToPlay;
        
        public Task<RoomResponse> CreateRoomAsync(string playerName)
        {
            return Task.FromResult(new RoomResponse()
            {
                Status = CreateRoomStatus.WaitingForAnotherPlayer,
                PlayerId = Guid.NewGuid(),
                RoomId = Guid.NewGuid(),
                AnotherPlayerName = nameof(RoomResponse.AnotherPlayerName)
            });
        }

        public Task<RoomResponse> GetRoomStatusAsync(Guid roomId, Guid playerId)
        {
            countTryReadyToPlay++;
            var status = countTryReadyToPlay > 3
                ? CreateRoomStatus.ReadyForStart
                : CreateRoomStatus.WaitingForAnotherPlayer;
            
            return Task.FromResult(new RoomResponse()
            {
                Status = status,
                PlayerId = playerId,
                RoomId = roomId,
                AnotherPlayerName = nameof(RoomResponse.AnotherPlayerName)
            });
        }

        public Task<GameStatusResponse> GetGameStatusAsync(Guid roomId, Guid playerId)
        {
            return Task.FromResult(new GameStatusResponse()
            {
                MyMap = { }
            });
        }

        public Task<GameFireResponse> FireAsync(Guid roomId, Guid playerId, CellPosition firedCell)
        {
            return Task.FromResult(new GameFireResponse()
            {
                OpponentMap = { }
            });
        }
    }
}