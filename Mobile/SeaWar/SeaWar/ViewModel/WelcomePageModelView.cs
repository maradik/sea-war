using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeaWar.Annotations;
using Xamarin.Forms;

namespace SeaWar.ViewModel
{
    public class WelcomePageModelView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string userName;

        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged(nameof(UserName));
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
    }
}