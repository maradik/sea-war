using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaWar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SeaWar.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaitGamePage : ContentPage
    {
        public WaitGamePage(WaitGamePageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}