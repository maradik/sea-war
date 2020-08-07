using Autofac;
using SeaWar.Client;
using SeaWar.View;
using SeaWar.ViewModels;

namespace SeaWar
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
                   {
                       var logger = context.Resolve<ILogger>();
                       return new RetryableClient(new Client.Client(Settings.ServerUri, Settings.Timeout, logger),
                                                  5);
                   })
                   .As<IClient>()
                   .SingleInstance();

            builder.RegisterType<Logger>()
                   .As<ILogger>()
                   .WithParameters(new[]
                   {
                       new NamedParameter("context", "SeaWar")
                   })
                   .SingleInstance();

            builder.RegisterType<WelcomePage>().AsSelf();
            builder.RegisterType<WelcomePageViewModel>().AsSelf();

            builder.RegisterType<MainMenuPage>().AsSelf();
            builder.RegisterType<MainMenuPageViewModel>().AsSelf();

            builder.RegisterType<WaitGamePage>().AsSelf();
            builder.RegisterType<WaitGamePageViewModel>().AsSelf();

            builder.RegisterType<GamePage>().AsSelf();
            builder.RegisterType<GameViewModel>().AsSelf();

            builder.RegisterType<FinishPage>().AsSelf();
            builder.RegisterType<FinishViewModel>().AsSelf();
        }
    }
}