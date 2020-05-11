using System;
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
        }
    }
}