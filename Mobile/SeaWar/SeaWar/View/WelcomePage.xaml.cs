using System;
using SeaWar.DomainModels;
using SeaWar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SeaWar.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage(GameModel model, Func<GameModel, WelcomePageViewModel> createViewModel)
        {
            InitializeComponent();
            BindingContext = createViewModel(model);
        }

        private WelcomePageViewModel Model => (BindingContext as WelcomePageViewModel);
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Model.RestorePlayerName();
        }
    }
}