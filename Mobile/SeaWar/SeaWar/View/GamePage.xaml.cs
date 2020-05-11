using System;
using System.ComponentModel;
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

        private void NotifyPropertyChangedOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.OpponentMap))
            {
                Model.UpdateGrid(AnotherPlayerGrid, Model.OpponentMap);    
            }
            
            if (e.PropertyName == nameof(Model.MyMap))
            {
                Model.UpdateGrid(PlayerGrid, Model.MyMap);    
            }
        }
    }
}