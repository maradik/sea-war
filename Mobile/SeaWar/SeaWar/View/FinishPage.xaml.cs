﻿using System;
using SeaWar.DomainModels;
using SeaWar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SeaWar.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FinishPage : ContentPage
    {
        public FinishPage(GameModel model, Func<GameModel, FinishViewModel> createViewModel)
        {
            InitializeComponent();
            BindingContext = createViewModel(model);
        }

        private FinishViewModel Model => (FinishViewModel) BindingContext;

        protected override bool OnBackButtonPressed()
        {
            Model.RestartGame.Execute(null);
            return true;
        }
    }
}