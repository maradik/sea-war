using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaWar.ViewModels;
using Xamarin.Forms;

namespace SeaWar
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