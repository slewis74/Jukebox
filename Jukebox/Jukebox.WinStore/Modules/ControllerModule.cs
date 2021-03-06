﻿using System.Reflection;
using Autofac;
using Jukebox.WinStore.Common;
using Orienteer.Pages;

namespace Jukebox.WinStore.Modules
{
    public class ControllerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<JukeboxControllerFactory>()
                .As<IControllerFactory>()
                .InstancePerLifetimeScope();

            builder
                .RegisterAssemblyTypes(typeof(ControllerModule).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<IController>())
                .AsSelf()
                .InstancePerDependency();
            builder
                .RegisterAssemblyTypes(typeof(ControllerModule).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<IController>())
                .As<IController>()
                .InstancePerDependency();

            builder.RegisterType<ControllerLocator>().As<IControllerLocator>().SingleInstance();
            builder.RegisterType<ControllerInvoker>().As<IControllerInvoker>().InstancePerDependency();
            builder.RegisterType<ControllerRouteConverter>().As<IControllerRouteConverter>().InstancePerDependency();
        }
    }
}