using Autofac;
using Jukebox.WinStore.Features.MainPage;

namespace Jukebox.WinStore.Modules
{
    public class ViewModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NowPlayingHeaderView>().AsSelf().InstancePerDependency();
        }
    }
}