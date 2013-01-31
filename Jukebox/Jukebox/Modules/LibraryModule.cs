using Autofac;
using Jukebox.Storage;

namespace Jukebox.Modules
{
    public class LibraryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MusicProvider>().As<IMusicProvider>().SingleInstance();
        }
    }
}