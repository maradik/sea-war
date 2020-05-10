using Autofac;
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
            MainPage = new NavigationPage(Container.Resolve<WelcomePage>());
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