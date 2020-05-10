using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SeaWar.Annotations;
using SeaWar.Client;
using SeaWar.Model;

namespace SeaWar.ViewModel
{
    public class WaitGamePageViewModel
    {
        private readonly IClient client;
        private readonly GameModel gameModel;
        private Task waitAnotherPlayerTask;
        private int millisecondsForRepeatServerRequest = 2 * 1000;
        
        public WaitGamePageViewModel(IClient client, GameModel gameModel)
        {
            this.client = client;
            this.gameModel = gameModel;
            waitAnotherPlayerTask = Task.Run(async () => await WaitGameReadyAsync());
        }

        private async Task WaitGameReadyAsync()
        {
            
        }
    }
}