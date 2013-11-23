using Autofac;
using SlabRt.Data.Navigation;
using SlabRt.Pages;
using SlabRt.Pages.Navigation;

namespace Jukebox.Modules
{
    public class NavigationModule : Module
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
            
            builder.RegisterType<ViewLocator>().As<IViewLocator>().SingleInstance();
        }
    }
}