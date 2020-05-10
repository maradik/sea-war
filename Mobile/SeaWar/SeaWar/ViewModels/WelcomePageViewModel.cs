using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeaWar.Annotations;
using SeaWar.Client;
using SeaWar.Client.Contracts;
using SeaWar.Validation;
using SeaWar.View;
using Xamarin.Forms;

namespace SeaWar.ViewModels
{
    public class WelcomePageModelView : INotifyPropertyChanged, IUseValidation
    {
        private readonly IClient client;
        
        private const int minUserNameLength = 5;
        private const string validateMessage = "Имя игрока должно быть не меньше 5 символов";
        public event PropertyChangedEventHandler PropertyChanged;

        private string _playerName;
        private bool isValid;
        private string errorMessage;
        
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

        public Command StartGame { get; }

        public WelcomePageModelView()
        {
            this.client = new FakeClient();
            
            StartGame = new Command(async _ =>
            {
                var createRoomResponse = await client.CreateRoomAsync(PlayerName);
                var gameModel = new GameModel()
                {
                    PlayerName = PlayerName,
                    PlayerId = createRoomResponse.PlayerId,
                    RoomId = createRoomResponse.RoomId,
                    AnotherPlayerName = createRoomResponse.AnotherPlayerName
                };
                
                if (createRoomResponse.Status == CreateRoomStatus.ReadyForStart)
                {
                    var mainPage = new MainPage
                    {
                        BindingContext = new GameViewModel(client, gameModel)
                    };
                    await Application.Current.MainPage.Navigation.PushAsync(mainPage);
                }
                else
                {
                    var waitPage = new WaitGamePage
                    {
                        BindingContext = new WaitGamePageViewModel(client, gameModel)
                    };
                    await Application.Current.MainPage.Navigation.PushAsync(waitPage);
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

        void IUseValidation.Validate()
        {
            IsValid = !IsEmptyOrSmall();
            ErrorMessage = !IsValid ? validateMessage : string.Empty;
        }
    }
}