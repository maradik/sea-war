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
            var fireResult = await client.FireAsync(gameModel.RoomId, gameModel.PlayerId, new CellPosition(x, y));
            OpponentMap = fireResult.OpponentMap.ToModel();

            //ToDo redraw
            await GetStatusAsync();
        }

        private async Task GetStatusAsync()
        {
            while (true)
            {
                var gameStatus = await client.GetGameStatusAsync(gameModel.RoomId, gameModel.PlayerId);

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
            var x = random.Next(OpponentMap.Cells.GetLength(0));
            var y = random.Next(OpponentMap.Cells.GetLength(1));
            await FireAsync(x, y);
        }
    }
}