using System;
using Autofac;
using SeaWar.DomainModels;
using SeaWar.View;
using SeaWar.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SeaWar
{
    public partial class App : Application
    {
        public IContainer Container { get; }
        
        public App()
        {
            InitializeComponent();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<AutofacModule>();
            Container = containerBuilder.Build();

            var gameModel = new GameModel();
            var createWelcomePage = Container.Resolve<Func<GameModel, WelcomePage>>();
            MainPage = new NavigationPage(createWelcomePage(gameModel));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}