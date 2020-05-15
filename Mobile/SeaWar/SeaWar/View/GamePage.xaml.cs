using System;
using System.ComponentModel;
using Autofac;
using SeaWar.DomainModels;
using SeaWar.ViewModels;
using Xamarin.Forms;

namespace SeaWar.View
{
    public partial class GamePage : ContentPage
    {
        public GamePage(GameModel model, Func<GameModel, GameViewModel> createViewModel)
        {
            InitializeComponent();
            BindingContext = createViewModel(model);
            
            var notifyPropertyChanged = Model as INotifyPropertyChanged;
            notifyPropertyChanged.PropertyChanged += NotifyPropertyChangedOnPropertyChanged;
        }

        GameViewModel Model =>  (BindingContext as GameViewModel);      
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Model.InitGrid(PlayerGrid, false);
            Model.InitGrid(AnotherPlayerGrid, true);
            Model.UpdateGrid(PlayerGrid, Model.MyMap);
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await DisplayAlert("Морской бой", "Закончить игру?", "Да", "Нет");
                if (result)
                {
                    base.OnBackButtonPressed();
                    var gameModel = new GameModel();
                    var createWelcomePage = ((App) Application.Current).Container.Resolve<Func<GameModel, WelcomePage>>();
                    var welcomePage = createWelcomePage(gameModel);
                    await Application.Current.MainPage.Navigation.PushAsync(welcomePage);
                }
            });

            return true;
        }

        private void NotifyPropertyChangedOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.OpponentMap))
            {
                Device.BeginInvokeOnMainThread(() =>
                    Model.UpdateGrid(AnotherPlayerGrid, Model.OpponentMap));
            }

            if (e.PropertyName == nameof(Model.MyMap))
            {
                Device.BeginInvokeOnMainThread(() => Model.UpdateGrid(PlayerGrid, Model.MyMap));
            }
        }
    }
}