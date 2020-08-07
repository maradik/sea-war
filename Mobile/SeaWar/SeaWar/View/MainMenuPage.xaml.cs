using System;
using System.Threading.Tasks;
using SeaWar.DomainModels;
using SeaWar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SeaWar.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage(GameModel model, Func<GameModel, MainMenuPageViewModel> createViewModel)
        {
            InitializeComponent();
            BindingContext = createViewModel(model);
        }
        
        private MainMenuPageViewModel Model => BindingContext as MainMenuPageViewModel;

        private async void OnRoomSelected(object sender, ItemTappedEventArgs e)
        {
            await Model.JoinRoom((Room) e.Item, () =>
            {
                Device.BeginInvokeOnMainThread(async () => {
                    await DisplayAlert("Морской бой", "Не удалось подключиться к игре", "ok").ConfigureAwait(true);
                });
                return Task.CompletedTask;
            }).ConfigureAwait(true);
        }
        
        protected override bool OnBackButtonPressed()
        {
            Model.RestartGame.Execute(null);
            return true;
        }
    }
}