using System.Reflection;
using Autofac;
using Slab.WinStore.Data.Navigation;
using Slab.WinStore.Pages;
using Slab.WinStore.Pages.Navigation;

namespace Jukebox.WinStore.Modules
{
    public class NavigationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<NavigationStackStorage>()
                .As<INavigationStackStorage>()
                .WithParameter("defaultRoute", "Artists/ShowAll")
                .WithParameter("alwaysStartFromDefaultRoute", false)
                .InstancePerLifetimeScope();

            builder
                .RegisterType<RtNavigator>().AsImplementedInterfaces()
                .SingleInstance();
            
            builder
                .RegisterType<ViewLocator>()
                .As<IViewLocator>()
                .SingleInstance()
                .OnActivated(x => x.Instance.Configure(typeof(NavigationModule).GetTypeInfo().Assembly, "Jukebox.WinStore.Features", "Jukebox.WinStore.Features"));
        }
    }
}