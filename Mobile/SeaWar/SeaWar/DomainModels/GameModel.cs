using System;
using System.Threading;
using SeaWar.ViewModels;
using Xamarin.Forms;

namespace SeaWar.DomainModels
{
    public class GameModel
    {
        private readonly Lazy<Guid> lazyPlayerId;
        public static int MapHorizontalSize = 10;
        public static int MapVerticalSize = 10;

        public GameModel() =>
            lazyPlayerId = new Lazy<Guid>(GetPlayerId, LazyThreadSafetyMode.ExecutionAndPublication);

        public string PlayerName { get; set; }
        public string AnotherPlayerName { get; set; }
        public Guid PlayerId => lazyPlayerId.Value;
        public Guid RoomId { get; set; }
        public FinishReason FinishReason { get; set; }
        public Map MyMap { get; set; }
        public Map OpponentMap { get; set; }
        
        private static Guid GetPlayerId()
        {
            if (!Application.Current.Properties.TryGetValue(nameof(PlayerId), out var playerIdString) || !Guid.TryParse(playerIdString as string, out var playerId))
            {
                playerId = Guid.NewGuid();
                Application.Current.Properties[nameof(PlayerId)] = playerId.ToString();
            }

            return playerId;
        }
    }
}