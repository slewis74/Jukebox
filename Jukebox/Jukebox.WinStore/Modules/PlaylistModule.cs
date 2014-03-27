using Autofac;
using Jukebox.WinStore.Storage;

namespace Jukebox.WinStore.Modules
{
    public class PlaylistModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<PlaylistHandler>()
                .AsSelf()
                .SingleInstance();

        }
    }
}