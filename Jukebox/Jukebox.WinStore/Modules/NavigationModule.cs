using System.Reflection;
using Windows.UI.Xaml;
using Autofac;
using Slab.WinStore.Data.Navigation;
using Slab.WinStore.Pages;
using Slab.WinStore.Pages.Navigation;
using Slab.Xaml;

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
                .As<IViewLocator<FrameworkElement>>()
                .SingleInstance()
                .OnActivated(x => x.Instance.Configure(typeof(NavigationModule).GetTypeInfo().Assembly, "PortableClassLibrary1.Features", "PhoneApp1.Features"));
        }
    }
}