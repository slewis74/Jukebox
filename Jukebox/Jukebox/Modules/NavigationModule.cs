﻿using Autofac;
using Slew.WinRT.Data.Navigation;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;

namespace Jukebox.Modules
{
    public class NavigationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NavigationStackStorage>().As<INavigationStackStorage>().InstancePerLifetimeScope();
            builder.RegisterType<Navigator>().As<INavigator>().SingleInstance();
            
            builder.RegisterType<ViewLocator>().As<IViewLocator>().SingleInstance();
        }
    }
}