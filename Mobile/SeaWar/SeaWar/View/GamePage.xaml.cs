using System;
using SeaWar.Client.Contracts;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var image = new Image()
                    {
                        Source = "empty_cell.jpg"
                    };

                    var tapGestureRecognizer = new TapGestureRecognizer();
                    var cellPosition = new CellPosition(i, j);
                    tapGestureRecognizer.Tapped += (s, e) => {
                        // handle the tap
                        var x = cellPosition.X;
                        var y = cellPosition.Y;
                        var tapImage = (Image)s;
                        tapImage.Source = ImageSource.FromFile("miss_cell.jpg");
                        int a = 10;
                    };
                    image.GestureRecognizers.Add(tapGestureRecognizer);
                    PlayerGrid.Children.Add(image,i,j);
                }
            }
        }
    }
}