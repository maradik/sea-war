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
        private readonly IClient client;
        private string status;
        private string formattedTimeout;

        public GameViewModel(IClient client) =>
            this.client = client;

        public event PropertyChangedEventHandler PropertyChanged;

        private Guid RoomId { get; }
        private Guid PlayerId { get; }
        public string MyName { get; }
        public string OpponentName { get; }
        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string FormattedTimeout
        {
            get => formattedTimeout;
            set
            {
                formattedTimeout = value;
                OnPropertyChanged(nameof(FormattedTimeout));
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

            Task.Run(GetStatus).ContinueInParallel();
        }

        private async Task GetStatus()
        {
            while (true)
            {
                var gameStatus = await client.GetGameStatusAsync(RoomId, PlayerId);

                switch (gameStatus.GameStatus)
                {
                    case GameStatus.YourChoise:
                        MyMap = gameStatus.MyMap.ToModel();
                        Status = "Твой ход";
                        return;
                    case GameStatus.PendingForFriendShips:
                        Status = "Расстановка кораблей соперника"; //TODO этого статуса быть не должно по логике
                        FormattedTimeout = string.Empty;
                        break;
                    case GameStatus.PendingForFriendChoise:
                        Status = "Ход соперника";
                        FormattedTimeout = string.Empty;
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