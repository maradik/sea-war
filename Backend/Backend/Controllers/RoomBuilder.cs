using System;
using Backend.Models;

namespace Backend.Controllers
{
    public class RoomBuilder
    {
        public Room Build(Player firstPlayer, Player secondPlayer)
        {
            if (firstPlayer == null && secondPlayer == null)
                throw new ArgumentException($"{nameof(firstPlayer)} and {nameof(secondPlayer)} cannot be null simultaneously");

            return new Room
            {
                Id = Guid.NewGuid(),
                Player1 = firstPlayer,
                Player2 = secondPlayer,
                Status = firstPlayer != null && secondPlayer != null
                    ? RoomStatus.Ready
                    : RoomStatus.NotReady,
                CurrentPlayerId = firstPlayer?.Id ?? secondPlayer.Id
            };
        }
    }
}