using Autofac;
using PresentationBus;

namespace Jukebox.WinStore.Modules
{
    public class PresentationBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PresentationBus.PresentationBus>()
                .As<IPresentationBus>()
                .As<IPresentationBusConfiguration>()
                .SingleInstance();
        }
    }
}