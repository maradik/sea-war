using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SeaWar.Annotations;
using SeaWar.Client;
using SeaWar.Client.Contracts;
using SeaWar.DomainModels;
using SeaWar.Extensions;
using SeaWar.View;
using Xamarin.Forms;

namespace SeaWar.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private static readonly Random random = new Random();
        private static readonly TimeSpan fireTimeout = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan periodOfStatusPolling = TimeSpan.FromSeconds(1);

        private ImageSource danageImageSource = ImageSource.FromFile("damage.jpg");
        private ImageSource emptyImageSource = ImageSource.FromFile("empty_cell.jpg");
        private ImageSource shipImageSource = ImageSource.FromFile("ship_cell.jpg");
        private ImageSource missImageSource = ImageSource.FromFile("miss_cell.jpg");

        private readonly IClient client;
        private readonly PeriodicalTimer fireTimeoutTimer;
        private string formattedStatus;
        private readonly GameModel gameModel;
        private readonly Func<GameModel, FinishPage> createFinishPage;
        private readonly CancellationTokenSource pageCancellationTokenSource = new CancellationTokenSource();

        private Map myMap;
        private Map opponentMap;
        private bool enabledOpponentGrid;

        public Map OpponentMap
        {
            get => opponentMap;
            set
            {
                opponentMap = value;
                OnPropertyChanged(nameof(OpponentMap));
            }
        }

        public Map MyMap
        {
            get => myMap;
            set
            {
                myMap = value;
                OnPropertyChanged(nameof(MyMap));
            }
        }

        public bool EnabledOpponentGrid
        {
            get => enabledOpponentGrid;
            set
            {
                enabledOpponentGrid = value;
                OnPropertyChanged(nameof(EnabledOpponentGrid));
            }
        }
        
        public GameViewModel(GameModel gameModel, Func<GameModel, FinishPage> createFinishPage, IClient client)
        {
            this.client = client;
            this.gameModel = gameModel;
            this.createFinishPage = createFinishPage;
            fireTimeoutTimer = new PeriodicalTimer(TimeSpan.FromSeconds(1), UpdateYourChoiceFormattedStatusAsync, fireTimeout, RandomFireAsync, SetOpponentChoiceFormattedStatusAsync);
            OpponentMap = Map.Empty;
            MyMap = Map.Empty;
            RestartGame = new Command(_ =>
            {
                pageCancellationTokenSource.Cancel();
                var application = (App)Application.Current;
                application.BeginGame();
            });

            Task.Run(async () => await GetStatusAsync());
            SetOpponentChoiceFormattedStatusAsync();
        }

        public Command RestartGame { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public string MyName => gameModel.PlayerName;
        public string OpponentName => gameModel.AnotherPlayerName;

        public string FormattedStatus
        {
            get => formattedStatus;
            set
            {
                formattedStatus = value;
                OnPropertyChanged(nameof(FormattedStatus));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task FireAsync(int x, int y)
        {
            EnabledOpponentGrid = false;
            await fireTimeoutTimer.StopAsync();

            //TODO нужно "запретить" стрелять по клетке, по которой уже стрелял
            var parameters = new FireParameters
            {
                RoomId = gameModel.RoomId,
                PlayerId = gameModel.PlayerId,
                FieredCell = new CellPosition(x, y)
            };
            var fireResult = await client.FireAsync(parameters);
            OpponentMap = fireResult.EnemyMap.ToModel();

            //ToDo redraw
            await GetStatusAsync();
        }

        private async Task GetStatusAsync()
        {
            while (!pageCancellationTokenSource.Token.IsCancellationRequested)
            {
                var parameters = new GetGameStatusParameters
                {
                    RoomId = gameModel.RoomId,
                    PlayerId = gameModel.PlayerId
                };

                var gameStatus = await client.GetGameStatusAsync(parameters);
                if (gameStatus.MyMap != null)
                {
                    MyMap = gameStatus.MyMap.ToModel();
                }

                switch (gameStatus.GameStatus)
                {
                    case GameStatus.YourChoice:
                        await fireTimeoutTimer.StartAsync(pageCancellationTokenSource.Token);
                        EnabledOpponentGrid = true;
                        return;
                    case GameStatus.PendingForFriendChoice:
                        break;
                    case GameStatus.Finish:
                        gameModel.MyMap = MyMap;
                        gameModel.OpponentMap = OpponentMap;
                        gameModel.FinishReason = gameStatus.FinishReason.ToModel();
                        Device.BeginInvokeOnMainThread(async () => {
                            await Application.Current.MainPage.Navigation.PushModalAsync(createFinishPage(gameModel));
                        });
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await Task.Delay(periodOfStatusPolling);
            }
        }

        private Task UpdateYourChoiceFormattedStatusAsync(TimeSpan remain)
        {
            FormattedStatus = $"Ходи {remain.TotalSeconds.ToString("0")}";
            return Task.CompletedTask;
        }

        private async Task RandomFireAsync()
        {
            //TODO нужно "запретить" стрелять по клетке, по которой уже стрелял
            var x = random.Next(OpponentMap.Cells.GetLength(0));
            var y = random.Next(OpponentMap.Cells.GetLength(1));

            await FireAsync(x, y);
        }

        private Task SetOpponentChoiceFormattedStatusAsync()
        {
            FormattedStatus = "Ход соперника";
            return Task.CompletedTask;
        }

        public void InitGrid(Grid grid, bool useTapAction)
        {
            for (int i = 0; i < GameModel.MapHorizontalSize; i++)
            {
                for (int j = 0; j < GameModel.MapVerticalSize; j++)
                {
                    var image = new Image()
                    {
                        Source = emptyImageSource
                    };

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    var cellPosition = new CellPosition(i, j);

                    if (useTapAction)
                    {
                        tapGestureRecognizer.Tapped += async (sender, eventArgs) =>
                        {
                            // handle the tap
                            var x = cellPosition.X;
                            var y = cellPosition.Y;
                            var tapImage = (Image) sender;
                            
                            //если кликаем не по пустой ячейке, то ничего не делаем
                            if (!IsEqualsImageSources(tapImage.Source, emptyImageSource))
                            {
                                return;
                            }
                            
                            await FireAsync(x, y);
                        };
                    }

                    image.GestureRecognizers.Add(tapGestureRecognizer);
                    grid.Children.Add(image, i, j);
                }
            }
        }

        public void UpdateGrid(Grid grid, Map map)
        {
            if (grid.Children.Count == 0 || grid.Children.Count < GameModel.MapHorizontalSize * GameModel.MapVerticalSize)
            {
                return;
            }
            
            var cells = map.Cells;
            for (int i = 0; i < GameModel.MapHorizontalSize; i++)
            {
                for (int j = 0; j < GameModel.MapVerticalSize; j++)
                {
                    var cell = cells[i, j];
                    var positionFlat = i * GameModel.MapVerticalSize + j;
                    var child = grid.Children[positionFlat];
                    var image = (Image) child;

                    var imageSource = emptyImageSource;
                    switch (cell.Status)
                    {
                        case CellStatus.Damaged:
                            imageSource = danageImageSource;
                            break;
                        case CellStatus.Filled:
                            imageSource = shipImageSource;
                            break;
                        case CellStatus.Missed:
                            imageSource = missImageSource;
                            break;
                        default:
                            imageSource = emptyImageSource;
                            break;
                    }

                    if (!IsEqualsImageSources(image.Source, imageSource))
                    {
                        image.Source = imageSource;
                    }
                }
            }
        }

        private bool IsEqualsImageSources(ImageSource sourceA, ImageSource sourceB)
        {
            return (((FileImageSource) sourceA).File == ((FileImageSource) sourceB).File);
        }
    }
}