using System.Reflection;
using System.Windows.Input;
using Autofac;
using Slab.WinStore.Commands;

namespace Jukebox.WinStore.Modules
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

            builder
                .RegisterAssemblyTypes(typeof (ViewModelModule).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<ICommand>())
                .AsSelf()
                .InstancePerDependency();
        }
    }
}