using Autofac;
using System.Reflection;
using Jukebox.Common;
using Slab.Pages;

namespace Jukebox.Modules
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
        }
    }
}