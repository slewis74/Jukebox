using System.Reflection;
using Windows.UI.Xaml;
using Autofac;
using Orienteer.Autofac;
using Orienteer.Pages.Navigation;
using Orienteer.WinStore.Data.Navigation;
using Orienteer.WinStore.Pages;
using Orienteer.WinStore.Pages.Navigation;

namespace Jukebox.WinStore.Modules
{
    public class NavigationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<WinStoreNavigationStackStorage>()
                .As<INavigationStackStorage>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<NavigationStack>()
                .As<INavigationStack>()
                .WithParameter("defaultRoute", "Artists/ShowAll")
                .WithParameter("alwaysStartFromDefaultRoute", false)
                .OnActivated(c => c.Context.InjectUnsetProperties(c.Instance))
                .InstancePerLifetimeScope();

            builder
                .RegisterType<RtNavigator>().AsImplementedInterfaces()
                .SingleInstance();
            
            builder
                .RegisterType<AutofacViewFactory<FrameworkElement>>()
                .AsImplementedInterfaces()
                .SingleInstance();
            
            builder
                .RegisterType<ViewLocator>()
                .AsImplementedInterfaces()
                .SingleInstance()
                .OnActivated(x => x.Instance.Configure(typeof(NavigationModule).GetTypeInfo().Assembly, "Jukebox.WinStore.Features", "Jukebox.WinStore.Features"));
        }
    }
}