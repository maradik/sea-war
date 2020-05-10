using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeaWar.Annotations;
using Xamarin.Forms;

namespace SeaWar.ViewModel
{
    public class WelcomePageModelView : INotifyPropertyChanged, IUseValidation
    {
        private const int minUserNameLength = 5;
        private const string validateMessage = "Имя игрока должно быть не меньше 5 символов";
        public event PropertyChangedEventHandler PropertyChanged;

        private string userName;
        private bool isValid;
        private string errorMessage;
        
        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged(nameof(UserName));
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
            StartGame = new Command(async _ =>
            {
                var mainPage = new MainPage();
                await Application.Current.MainPage.Navigation.PushAsync(mainPage);
            });
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool IsEmptyOrSmall()
        {
            return string.IsNullOrEmpty(UserName) || UserName.Length < minUserNameLength;
        }

        void IUseValidation.Validate()
        {
            IsValid = !IsEmptyOrSmall();
            ErrorMessage = !IsValid ? validateMessage : string.Empty;
        }
    }
}