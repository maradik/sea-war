using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeaWar.DomainModels;
using SeaWar.View;
using Xamarin.Forms;

namespace SeaWar.ViewModels
{
    public class FinishViewModel : INotifyPropertyChanged
    {
        private string formattedReason;
        private int myDamagesCount;
        private int myMissesCount;
        private int opponentDamagesCount;
        private int opponentMissesCount;

        public FinishViewModel(GameModel model)
        {
            FormattedReason = ToFormattedReason(model.FinishReason);
            (OpponentDamagesCount, OpponentMissesCount) = GetStatistics(model.MyMap);
            (MyDamagesCount, MyMissesCount) = GetStatistics(model.OpponentMap);

            RestartGame = new Command(_ =>
            {
                var application = (App)Application.Current;
                application.BeginGame();
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Command RestartGame { get; }

        public string FormattedReason
        {
            get => formattedReason;
            set
            {
                formattedReason = value;
                OnPropertyChanged(nameof(FormattedReason));
            }
        }

        public int MyDamagesCount
        {
            get => myDamagesCount;
            set
            {
                myDamagesCount = value;
                OnPropertyChanged(nameof(MyDamagesCount));
            }
        }

        public int MyMissesCount
        {
            get => myMissesCount;
            set
            {
                myMissesCount = value;
                OnPropertyChanged(nameof(MyMissesCount));
            }
        }

        public int OpponentDamagesCount
        {
            get => opponentDamagesCount;
            set
            {
                opponentDamagesCount = value;
                OnPropertyChanged(nameof(OpponentDamagesCount));
            }
        }

        public int OpponentMissesCount
        {
            get => opponentMissesCount;
            set
            {
                opponentMissesCount = value;
                OnPropertyChanged(nameof(OpponentMissesCount));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static (int damagesCount, int missesCount) GetStatistics(Map map)
        {
            var damagesCount = 0;
            var missesCount = 0;
            foreach (var cell in map.Cells)
            {
                if (cell.Status == CellStatus.Damaged)
                {
                    damagesCount++;
                }

                if (cell.Status == CellStatus.Missed)
                {
                    missesCount++;
                }
            }

            return (damagesCount, missesCount);
        }

        private static string ToFormattedReason(FinishReason finishReason)
        {
            return finishReason switch
            {
                FinishReason.Winner => "УРА!!! ПОБЕДА!!!",
                FinishReason.Lost => "Ты проиграл!",
                FinishReason.OpponentConnectionLost => "Потеряно соединение с другим игроком :(",
                FinishReason.ConnectionLost => "Потеряно соединение с Интернетом :(",
                _ => throw new ArgumentException()
            };
        }
    }
}