using SeaWar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SeaWar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage(WelcomePageModelView viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private WelcomePageModelView Model => (BindingContext as WelcomePageModelView);
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Model.RestorePlayerName();
        }
    }
}