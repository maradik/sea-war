using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Integration.Dtos.v2;
using SeaWar.Annotations;
using SeaWar.Client;
using SeaWar.DomainModels;
using SeaWar.Extensions;
using SeaWar.View;
using Xamarin.Forms;

namespace SeaWar.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private static readonly Random random = new Random();
        private static readonly TimeSpan defaultFireTimeout = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan periodOfStatusRefresh = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan periodOfStatusPolling = TimeSpan.FromSeconds(1);

        private readonly IClient client;
        private readonly PeriodicalTimer fireTimeoutTimer;
        private readonly GameModel gameModel;
        private readonly Func<GameModel, FinishPage> createFinishPage;
        private readonly CancellationTokenSource pageCancellationTokenSource = new CancellationTokenSource();

        private readonly ImageSource danageImageSource = ImageSource.FromFile("damage.jpg");
        private readonly ImageSource emptyImageSource = ImageSource.FromFile("empty_cell.jpg");
        private readonly ImageSource shipImageSource = ImageSource.FromFile("ship_cell.jpg");
        private readonly ImageSource missImageSource = ImageSource.FromFile("miss_cell.jpg");
        private string formattedStatus;

        private Map myMap;
        private Map opponentMap;
        private bool enabledOpponentGrid;
        private Color statusColor;

        public GameViewModel(GameModel gameModel, Func<GameModel, FinishPage> createFinishPage, IClient client)
        {
            this.client = client;
            this.gameModel = gameModel;
            this.createFinishPage = createFinishPage;
            fireTimeoutTimer = new PeriodicalTimer(UpdateYourChoiceFormattedStatusAsync, RandomFireAsync, SetOpponentChoiceFormattedStatusAsync);
            OpponentMap = Map.Empty;
            MyMap = Map.Empty;
            RestartGame = new Command(_ =>
            {
                pageCancellationTokenSource.Cancel();
                var application = (App) Application.Current;
                application.BeginGame();
            });

            Task.Run(async () => await GetStatusAsync().ConfigureAwait(true));
            SetOpponentChoiceFormattedStatusAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public Command RestartGame { get; }
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

        public Color StatusColor
        {
            get => statusColor;
            set
            {
                statusColor = value;
                OnPropertyChanged(nameof(StatusColor));
            }
        }


        public void InitGrid(Grid grid, bool useTapAction)
        {
            for (var i = 0; i < GameModel.MapHorizontalSize; i++)
            {
                for (var j = 0; j < GameModel.MapVerticalSize; j++)
                {
                    var image = new Image
                    {
                        Source = emptyImageSource
                    };

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    var cellPosition = new CellPosition(i, j);

                    if (useTapAction)
                    {
                        tapGestureRecognizer.Tapped += async (sender, eventArgs) =>
                        {
                            var tapImage = (Image) sender;

                            //если кликаем не по пустой ячейке, то ничего не делаем
                            if (!IsEqualsImageSources(tapImage.Source, emptyImageSource))
                            {
                                return;
                            }

                            await FireAsync(cellPosition).ConfigureAwait(true);
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
            for (var i = 0; i < GameModel.MapHorizontalSize; i++)
            {
                for (var j = 0; j < GameModel.MapVerticalSize; j++)
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task FireAsync(CellPosition cellPosition)
        {
            EnabledOpponentGrid = false;
            await fireTimeoutTimer.StopAsync().ConfigureAwait(true);

            var fireRequestDto = new FireRequestDto{X = cellPosition.X, Y = cellPosition.Y};
            var fireResult = await client.FireAsync(fireRequestDto, gameModel.RoomId, gameModel.PlayerId).ConfigureAwait(true);
            OpponentMap = fireResult.EnemyMap.ToModel();

            await GetStatusAsync().ConfigureAwait(true);
        }

        private async Task GetStatusAsync()
        {
            while (!pageCancellationTokenSource.Token.IsCancellationRequested)
            {
                var game = await client.GetGameAsync(gameModel.RoomId, gameModel.PlayerId).ConfigureAwait(true);
                if (game.MyMap != null)
                {
                    MyMap = game.MyMap.ToModel();
                }

                switch (game.Status)
                {
                    case GameStatusDto.YourChoice:
                        await fireTimeoutTimer.StartAsync(periodOfStatusRefresh, game.YourChoiceTimeout == default ? defaultFireTimeout : game.YourChoiceTimeout, pageCancellationTokenSource.Token).ConfigureAwait(true);
                        EnabledOpponentGrid = true;
                        return;
                    case GameStatusDto.PendingForFriendChoice:
                        break;
                    case GameStatusDto.Finish:
                        gameModel.MyMap = MyMap;
                        gameModel.OpponentMap = OpponentMap;
                        gameModel.FinishReason = game.FinishReason.ToModel();
                        Device.BeginInvokeOnMainThread(async () => { await Application.Current.MainPage.Navigation.PushModalAsync(createFinishPage(gameModel)).ConfigureAwait(true); });
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await Task.Delay(periodOfStatusPolling).ConfigureAwait(true);
            }
        }

        private Task UpdateYourChoiceFormattedStatusAsync(TimeSpan remain)
        {
            FormattedStatus = $"Ходи! Осталось {remain.TotalSeconds:0} сек.";
            StatusColor = Color.Green;
            return Task.CompletedTask;
        }

        private async Task RandomFireAsync()
        {
            var emptyCellPositions = OpponentMap.GetCellPositionsWithStatus(CellStatus.Empty);
            if (!emptyCellPositions.Any())
            {
                // если пустых клеток нет, то игра должна была уже закончиться к этому моменту, пробуем еще раз получить статус игры с бэкенда
                await GetStatusAsync().ConfigureAwait(true);
                return;
            }

            var cellPositionIndex = random.Next(emptyCellPositions.Length);
            await FireAsync(emptyCellPositions[cellPositionIndex]).ConfigureAwait(true);
        }

        private Task SetOpponentChoiceFormattedStatusAsync()
        {
            FormattedStatus = "Ход соперника...";
            StatusColor = Color.Crimson;
            return Task.CompletedTask;
        }

        private bool IsEqualsImageSources(ImageSource sourceA, ImageSource sourceB) =>
            ((FileImageSource) sourceA).File == ((FileImageSource) sourceB).File;
    }
}