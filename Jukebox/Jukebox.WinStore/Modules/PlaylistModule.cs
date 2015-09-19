using Autofac;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Storage;

namespace Jukebox.WinStore.Modules
{
    public class PlaylistModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PlaylistData>()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<NowPlayingPlaylist>()
                .AsSelf()
                .InstancePerDependency();

            builder
                .RegisterType<PlaylistHandler>()
                .AsSelf()
                .SingleInstance();

        }
    }
}