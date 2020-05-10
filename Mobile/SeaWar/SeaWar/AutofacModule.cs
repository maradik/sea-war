using Autofac;
using SeaWar.Client;
using SeaWar.DomainModels;
using SeaWar.View;
using SeaWar.ViewModels;

namespace SeaWar
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FakeClient>().As<IClient>().SingleInstance();
            
            builder.RegisterType<GameModel>().AsSelf().SingleInstance();
            
            builder.RegisterType<WelcomePage>().AsSelf().SingleInstance();
            builder.RegisterType<WelcomePageModelView>().AsSelf().SingleInstance();
            
            builder.RegisterType<WaitGamePage>().AsSelf().SingleInstance();
            builder.RegisterType<WaitGamePageViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<GamePage>().AsSelf().SingleInstance();
            builder.RegisterType<GameViewModel>().AsSelf().SingleInstance();
        }
    }
}