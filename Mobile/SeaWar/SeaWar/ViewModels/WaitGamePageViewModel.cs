using System.Threading.Tasks;
using SeaWar.Client;
using SeaWar.DomainModels;

namespace SeaWar.ViewModels
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
            //TODO запускать поллинг не на создание модели, а через отдельный вызов на отображение экрана
//            waitAnotherPlayerTask = Task.Run(async () => await WaitGameReadyAsync());
        }

        private async Task WaitGameReadyAsync()
        {
            
        }
    }
}