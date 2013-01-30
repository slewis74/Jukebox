using Autofac;
using Jukebox.Storage;
using Slew.WinRT.Pages.Settings;

namespace Jukebox.Modules
{
    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SettingsManager>().As<ISettingsManager>().SingleInstance();
            builder.RegisterType<SettingsHandler>().AsSelf().SingleInstance();
        }
    }
}