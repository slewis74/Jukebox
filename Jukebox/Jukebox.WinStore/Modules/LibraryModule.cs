using Autofac;
using Jukebox.WinStore.Storage;

namespace Jukebox.WinStore.Modules
{
    public class LibraryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MusicProvider>().As<IMusicProvider>().SingleInstance();
            builder.RegisterType<AlbumArtStorage>().As<IAlbumArtStorage>().SingleInstance();
        }
    }
}