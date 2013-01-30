using Autofac;
using Jukebox.Storage;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Modules
{
    public class PlaylistModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PlaylistHandler>().AsSelf().SingleInstance()
                                                .OnActivated(x =>
                                                {
                                                    var bus = x.Context.Resolve<IPresentationBus>();
                                                    bus.Subscribe(x.Instance);
                                                });

        }
    }
}