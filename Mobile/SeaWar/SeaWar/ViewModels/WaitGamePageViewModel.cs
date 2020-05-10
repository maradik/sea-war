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
        private readonly GamePage gamePage;
        private Task waitAnotherPlayerTask;
        private int millisecondsForRepeatServerRequest = 2 * 1000;

        public WaitGamePageViewModel(IClient client, GameModel gameModel, GamePage gamePage)
        {
            this.client = client;
            this.gameModel = gameModel;
            this.gamePage = gamePage;
        }

        private async Task WaitGameReadyAsync()
        {
            while (true)
            {
                try
                {
                    var parameters = new GetRoomStatusParameters
                    {
                        RoomId = gameModel.RoomId,
                        PlayerId = gameModel.PlayerId
                    };
                    var getRoomStatusResponse = await client.GetRoomStatusAsync(parameters);
                    if (getRoomStatusResponse.Status == CreateRoomStatus.ReadyForStart)
                    {
                        Device.BeginInvokeOnMainThread(async () => {
                            await Application.Current.MainPage.Navigation.PushModalAsync(gamePage);
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

        public void StartWaitAnotherPlayer()
        {
            waitAnotherPlayerTask = Task.Run(async () => await WaitGameReadyAsync());            
        }
    }
}