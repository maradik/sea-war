using System;
using SeaWar.DomainModels;
using SeaWar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SeaWar.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaitGamePage : ContentPage
    {
        public WaitGamePage(GameModel model, Func<GameModel, WaitGamePageViewModel> createViewModel)
        {
            InitializeComponent();
            BindingContext = createViewModel(model);
        }

        private WaitGamePageViewModel Model => (WaitGamePageViewModel) BindingContext;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Model.StartWaitAnotherPlayer();
        }

        protected override bool OnBackButtonPressed()
        {
            Model.RestartGame.Execute(null);
            return true;
        }
    }
}