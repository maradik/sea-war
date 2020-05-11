using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        
        private WaitGamePageViewModel Model => (BindingContext as WaitGamePageViewModel);
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Model.StartWaitAnotherPlayer();
        }
    }
}