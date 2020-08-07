using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeaWar.Annotations;
using SeaWar.DomainModels;
using SeaWar.Validation;
using SeaWar.View;
using Xamarin.Forms;

namespace SeaWar.ViewModels
{
    public class WelcomePageViewModel : INotifyPropertyChanged, IUseValidation
    {
        private const int minUserNameLength = 5;
        private const string validateMessage = "Имя игрока должно быть не меньше 5 символов";

        public event PropertyChangedEventHandler PropertyChanged;

        private string _playerName;
        private bool isValid;
        private string errorMessage;
        private bool pageEnabled;

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged(nameof(PlayerName));
            }
        }

        public bool IsValid
        {
            get => isValid;
            set
            {
                isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool PageEnabled
        {
            get => pageEnabled;
            set
            {
                pageEnabled = value;
                OnPropertyChanged(nameof(PageEnabled));
            }
        }

        public Command StartGame { get; }

        public WelcomePageViewModel(GameModel gameModel, Func<GameModel, MainMenuPage> createMainMenuPage)
        {
            PageEnabled = true;
            StartGame = new Command(async _ =>
            {
                PageEnabled = false;
                SavePlayerName();
                gameModel.PlayerName = PlayerName;
                await Application.Current.MainPage.Navigation.PushModalAsync(createMainMenuPage(gameModel)).ConfigureAwait(true);
            });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool IsEmptyOrSmall()
        {
            return string.IsNullOrEmpty(PlayerName) || PlayerName.Length < minUserNameLength;
        }

        public void Validate()
        {
            IsValid = !IsEmptyOrSmall();
            ErrorMessage = !IsValid ? validateMessage : string.Empty;
        }

        public void RestorePlayerName()
        {
            if (Application.Current.Properties.TryGetValue(nameof(PlayerName), out var playerName))
            {
                PlayerName = playerName.ToString();
            }

            Validate();
        }

        private void SavePlayerName()
        {
            Application.Current.Properties[nameof(PlayerName)] = PlayerName;
        }
    }
}