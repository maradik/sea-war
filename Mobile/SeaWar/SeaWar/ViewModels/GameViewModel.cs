using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SeaWar.Client;
using SeaWar.Client.Contracts;
using SeaWar.DomainModels;
using SeaWar.Extensions;

namespace SeaWar.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private static readonly Random random = new Random();
        private static readonly TimeSpan fireTimeout = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan periodOfStatusPolling = TimeSpan.FromSeconds(1);

        private readonly IClient client;
        private readonly PeriodicalTimer fireTimeoutTimer;
        private string formattedStatus;
        private readonly GameModel gameModel;

        //TODO Для тестовых целей только!!! Настоящие карты лежат в MyMap и OpponentMap.
        //TODO Выпилить, когда в GamePage.xaml замкнем отрисовку на настоящие карты
        public Cell[][] Cells { get; } = {
            new[]{new Cell {Status = CellStatus.Damaged}, new Cell {Status = CellStatus.Empty}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell(), new Cell()},
        };

        public GameViewModel(GameModel gameModel, IClient client)
        {
            this.client = client;
            this.gameModel = gameModel;
            fireTimeoutTimer = new PeriodicalTimer(TimeSpan.FromSeconds(1), UpdateYourChoiceFormattedStatusAsync, fireTimeout, RandomFireAsync, SetOpponentChoiceFormattedStatusAsync);
            OpponentMap = Map.Empty;
            Task.Run(async () => await GetStatusAsync());
            SetOpponentChoiceFormattedStatusAsync();
        }

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

        public Map MyMap { get; set; }
        public Map OpponentMap { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task FireAsync(int x, int y)
        {
            //TODO Задизейблить контролы
            await fireTimeoutTimer.Stop();
            
            //TODO нужно "запретить" стрелять по клетке, по которой уже стрелял
            var parameters = new FireParameters
            {
                RoomId = gameModel.RoomId,
                PlayerId = gameModel.PlayerId,
                FieredCell = new CellPosition(x, y)
            };
            var fireResult = await client.FireAsync(parameters);
            OpponentMap = fireResult.OpponentMap.ToModel();

            //ToDo redraw
            await GetStatusAsync();
        }

        private async Task GetStatusAsync()
        {
            while (true)
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
                    case GameStatus.YourChoise:
                        await fireTimeoutTimer.Start();
                        //TODO Раздизейблить контролы
                        return;
                    case GameStatus.PendingForFriendChoise:
                        break;
                    case GameStatus.Finish:
                        //TODO GoTo Finish screen
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
    }
}