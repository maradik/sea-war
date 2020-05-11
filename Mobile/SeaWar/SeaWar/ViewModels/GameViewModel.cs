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
        private static readonly TimeSpan fireTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan periodOfStatusPolling = TimeSpan.FromSeconds(1);

        private readonly IClient client;
        private readonly PeriodicalTimer fireTimeoutTimer;
        private string formattedStatus;
        private string formattedTimeoutRemain;
        private readonly GameModel gameModel;

        //TODO Для тестовых целей только!!! Настоящие карты лежат в MyMap и OpponentMap.
        //TODO Выпилить, когда в GamePage.xaml замкнем отрисовку на настоящие карты
        public Cell[][] Cells { get; } = {
            new[]{new Cell {Status = CellStatus.Damaged}, new Cell {Status = CellStatus.Empty}, new Cell()},
            new[]{new Cell {Status = CellStatus.Missed}, new Cell {Status = CellStatus.Filled}, new Cell()}
        };

        public GameViewModel(IClient client, GameModel gameModel)
        {
            this.client = client;
            this.gameModel = gameModel;
            fireTimeoutTimer = new PeriodicalTimer(TimeSpan.FromSeconds(1), UpdateFormattedTimeoutRemainAsync, fireTimeout, RandomFireAsync);
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

        public string FormattedTimeoutRemain
        {
            get => formattedTimeoutRemain;
            set
            {
                formattedTimeoutRemain = value;
                OnPropertyChanged(nameof(FormattedTimeoutRemain));
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
            fireTimeoutTimer.Stop();
            
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

                switch (gameStatus.GameStatus)
                {
                    case GameStatus.YourChoise:
                        MyMap = gameStatus.MyMap.ToModel();
                        FormattedStatus = "Твой ход";
                        fireTimeoutTimer.Start();
                        //TODO Раздизейблить контролы
                        return;
                    case GameStatus.PendingForFriendChoise:
                        FormattedStatus = "Ход соперника";
                        FormattedTimeoutRemain = string.Empty;
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

        private Task UpdateFormattedTimeoutRemainAsync(TimeSpan remain)
        {
            FormattedTimeoutRemain = remain.ToString();
            return Task.CompletedTask;
        }

        private async Task RandomFireAsync()
        {
            FormattedTimeoutRemain = String.Empty;
            
            //TODO нужно "запретить" стрелять по клетке, по которой уже стрелял
            var x = random.Next(OpponentMap.Cells.GetLength(0));
            var y = random.Next(OpponentMap.Cells.GetLength(1));
            
            await FireAsync(x, y);
        }
    }
}