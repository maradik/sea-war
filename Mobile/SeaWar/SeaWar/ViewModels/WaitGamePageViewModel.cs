using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Integration.Dtos.v2;
using SeaWar.Client;
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
                    var room = await client.GetRoomAsync(gameModel.RoomId, gameModel.PlayerId).ConfigureAwait(true);
                    switch (room.Status)
                    {
                        case RoomStatusDto.Opened:
                            break;
                        case RoomStatusDto.Ready:
                            gameModel.AnotherPlayerName = room.Players.Single(x => x.Id != gameModel.PlayerId).Name;
                            Device.BeginInvokeOnMainThread(async () => {
                                await Application.Current.MainPage.Navigation.PushModalAsync(createGamePage(gameModel)).ConfigureAwait(true);
                            });
                            return;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(room.Status), room.Status, null);
                    }

                    await Task.Delay(millisecondsForRepeatServerRequest).ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    logger.Info(ex.ToString());
                }
            }
        }

        public void StartWaitAnotherPlayer()
        {
            Task.Run(async () => await WaitGameReadyAsync().ConfigureAwait(true));
        }
    }
}