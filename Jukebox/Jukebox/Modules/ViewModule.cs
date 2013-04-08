using Autofac;
using Jukebox.Features.MainPage;

namespace Jukebox.Modules
{
    public class ViewModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NowPlayingHeaderView>().AsSelf().InstancePerDependency();
        }
    }
}