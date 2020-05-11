using System;
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
            // builder.RegisterType<FakeClient>().As<IClient>().SingleInstance();
            builder.RegisterType<RetryableClient>()
                   .As<IClient>()
                   .WithParameters(new []
                   {
                       new NamedParameter("client", new Client.Client(Settings.ServerUri, Settings.Timeout)),
                       new NamedParameter("retryCount", 5)
                   })
                   .SingleInstance();

            builder.RegisterType<WelcomePage>().AsSelf();
            builder.RegisterType<WelcomePageModelView>().AsSelf();

            builder.RegisterType<WaitGamePage>().AsSelf();
            builder.RegisterType<WaitGamePageViewModel>().AsSelf();

            builder.RegisterType<GamePage>().AsSelf();
            builder.RegisterType<GameViewModel>().AsSelf();
        }
    }
}