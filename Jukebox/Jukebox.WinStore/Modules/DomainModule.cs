using System.Reflection;
using Autofac;
using Jukebox.WinStore.EventHandlers;
using Module = Autofac.Module;

namespace Jukebox.WinStore.Modules
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterType<EventBroker>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterAssemblyTypes(typeof(DomainModule).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<OnDemandEventHandler>() && t.GetTypeInfo().IsAbstract == false)
                .AsImplementedInterfaces()
                .InstancePerDependency();
        }
    }
}