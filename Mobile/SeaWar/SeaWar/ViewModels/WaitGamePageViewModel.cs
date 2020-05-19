using System;
using System.Threading;
using System.Threading.Tasks;
using SeaWar.Client;
using SeaWar.Client.Contracts;
using SeaWar.DomainModels;
using SeaWar.View;
using Xamarin.Forms;

namespace SeaWar.ViewModels
{
    public class WaitGamePageViewModel
    {
        private readonly IClient client;
        private readonly ILogger logger;
        private readonly GameModel gameModel;
        private readonly Func<GameModel, GamePage> createGamePage;
        private readonly CancellationTokenSource pageCancellationTokenSource = new CancellationTokenSource();
        private int millisecondsForRepeatServerRequest = 2 * 1000;

        public WaitGamePageViewModel(GameModel gameModel, Func<GameModel, GamePage> createGamePage, IClient client, ILogger logger)
        {
            this.client = client;
            this.logger = logger.WithContext(nameof(WaitGamePageViewModel));
            this.gameModel = gameModel;
            this.createGamePage = createGamePage;

            RestartGame = new Command(_ =>
            {
                pageCancellationTokenSource.Cancel();
                var application = (App)Application.Current;
                application.BeginGame();
            });
        }

        public Command RestartGame { get; }

        private async Task WaitGameReadyAsync()
        {
            while (!pageCancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    var parameters = new GetRoomStatusParameters
                    {
                        RoomId = gameModel.RoomId,
                        PlayerId = gameModel.PlayerId
                    };
                    var getRoomStatusResponse = await client.GetRoomStatusAsync(parameters);
                    if (getRoomStatusResponse.RoomStatus == CreateRoomStatus.Ready)
                    {
                        gameModel.AnotherPlayerName = getRoomStatusResponse.AnotherPlayerName;
                        Device.BeginInvokeOnMainThread(async () => {
                            await Application.Current.MainPage.Navigation.PushModalAsync(createGamePage(gameModel));
                        });

                        return;
                    }

                    await Task.Delay(millisecondsForRepeatServerRequest);
                }
                catch (Exception ex)
                {
                    logger.Info(ex.ToString());
                }
            }
        }

        public void StartWaitAnotherPlayer()
        {
            Task.Run(async () => await WaitGameReadyAsync());
        }
    }
}