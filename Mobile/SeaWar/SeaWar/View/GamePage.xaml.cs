using SeaWar.ViewModels;
using Xamarin.Forms;

namespace SeaWar.View
{
    public partial class GamePage : ContentPage
    {
        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}