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
        public WelcomePage(GameModel model, Func<GameModel, WelcomePageModelView> createViewModel)
        {
            InitializeComponent();
            BindingContext = createViewModel(model);
        }

        private WelcomePageModelView Model => (BindingContext as WelcomePageModelView);
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Model.RestorePlayerName();
        }
    }
}