using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeaWar.Annotations;
using SeaWar.Client;
using SeaWar.Client.Contracts;
using SeaWar.DomainModels;
using SeaWar.Validation;
using SeaWar.View;
using Xamarin.Forms;

namespace SeaWar.ViewModels
{
    public class WelcomePageModelView : INotifyPropertyChanged, IUseValidation
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

        public WelcomePageModelView(GameModel gameModel, Func<GameModel, WaitGamePage> createWaitGamePage, Func<GameModel, GamePage> createGamePage, IClient client)
        {
            PageEnabled = true;
            StartGame = new Command(async _ =>
            {
                PageEnabled = false;
                SavePlayerName();

                var parameters = new CreateRoomParameters
                {
                    PlayerName = PlayerName,
                    PlayerId = gameModel.PlayerId
                };
                var createRoomResponse = await client.CreateRoomAsync(parameters);
                gameModel.PlayerName = PlayerName;
                gameModel.RoomId = createRoomResponse.RoomId;
                gameModel.AnotherPlayerName = createRoomResponse.AnotherPlayerName;

                switch (createRoomResponse.RoomStatus)
                {
                    case CreateRoomStatus.NotReady:
                        await Application.Current.MainPage.Navigation.PushModalAsync(createWaitGamePage(gameModel));                        
                        break;
                    case CreateRoomStatus.Ready:
                        await Application.Current.MainPage.Navigation.PushModalAsync(createGamePage(gameModel));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(createRoomResponse.RoomStatus), createRoomResponse.RoomStatus, null);
                }
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