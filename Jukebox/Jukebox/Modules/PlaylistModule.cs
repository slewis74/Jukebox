using Autofac;
using Jukebox.Storage;

namespace Jukebox.Modules
{
    public class PlaylistModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PlaylistHandler>().AsSelf().SingleInstance();
        }
    }
}