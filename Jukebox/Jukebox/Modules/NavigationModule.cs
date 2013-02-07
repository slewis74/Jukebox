using Autofac;
using Jukebox.Common;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;

namespace Jukebox.Modules
{
    public class NavigationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JukeboxControllerFactory>().As<IControllerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Navigator>().As<INavigator>().SingleInstance();
            
            builder.RegisterType<ViewResolver>().As<IViewResolver>().SingleInstance();
        }
    }
}