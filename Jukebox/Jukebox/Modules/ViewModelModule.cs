using System.Reflection;
using Autofac;

namespace Jukebox.Modules
{
    public class ViewModelModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof (ViewModelModule).GetTypeInfo().Assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}