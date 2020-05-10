using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SeaWar.Client;
using SeaWar.Contracts;
using SeaWar.Extensions;

namespace SeaWar.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private static readonly TimeSpan ChoiсeTimeout = TimeSpan.FromSeconds(10);
        
        private readonly IClient client;
        private string formattedStatus;
        private string formattedTimeoutRemain;
        private TimeSpan timeoutRemain;

        public GameViewModel(IClient client) =>
            this.client = client;

        public event PropertyChangedEventHandler PropertyChanged;

        private Guid RoomId { get; }
        private Guid PlayerId { get; }
        public string MyName { get; }
        public string OpponentName { get; }
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
            var fireResult = await client.FireAsync(RoomId, PlayerId, new CellPosition(x, y));
            OpponentMap = fireResult.OpponentMap.ToModel();

            //ToDo redraw

            Task.Run(async () => GetStatusAsync()).ContinueInParallel();
        }

        private async Task GetStatusAsync()
        {
            while (true)
            {
                var gameStatus = await client.GetGameStatusAsync(RoomId, PlayerId);

                switch (gameStatus.GameStatus)
                {
                    case GameStatus.YourChoise:
                        MyMap = gameStatus.MyMap.ToModel();
                        FormattedStatus = "Твой ход";
                        return;
                    case GameStatus.PendingForFriendChoise:
                        FormattedStatus = "Ход соперника";
                        FormattedTimeoutRemain = string.Empty;
                        //TODO Задизейблить контролы
                        break;
                    case GameStatus.Finish:
                        //TODO GoTo Finish screen
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await Task.Delay(1000);
            }
        }
    }
}