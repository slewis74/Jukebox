using System.Reflection;
using Autofac;
using SlabRt.Commands;

namespace Jukebox.Modules
{
    public class ViewModelModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GoBackCommand>().AsSelf().InstancePerDependency();
            
            builder
                .RegisterAssemblyTypes(typeof (ViewModelModule).GetTypeInfo().Assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsSelf()
                .InstancePerDependency();
        }
    }
}