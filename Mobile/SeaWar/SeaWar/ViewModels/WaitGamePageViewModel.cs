using System;
using System.Threading.Tasks;
using SeaWar.Client;
using SeaWar.Client.Contracts;
using SeaWar.DomainModels;
using Xamarin.Forms;

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
            waitAnotherPlayerTask = Task.Run(async () => await WaitGameReadyAsync());
        }

        private async Task WaitGameReadyAsync()
        {
            while (true)
            {
                try
                {
                    var getRoomStatusResponse = await client.GetRoomStatusAsync(gameModel.RoomId, gameModel.PlayerId);
                    if (getRoomStatusResponse.Status == CreateRoomStatus.ReadyForStart)
                    {
                        var mainPage = new MainPage
                        {
                            BindingContext = new GameViewModel(client, gameModel)
                        };
                        
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => {
                            await Application.Current.MainPage.Navigation.PushModalAsync(mainPage);
                        });    
                        
                        return;
                    }

                    await Task.Delay(millisecondsForRepeatServerRequest);
                }
                catch (Exception ex)
                {
                    //TODO logging 
                }
            }
        }
    }
}