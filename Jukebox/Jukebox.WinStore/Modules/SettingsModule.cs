using Autofac;
using Jukebox.WinStore.Features.Settings;
using Jukebox.WinStore.Storage;
using Orienteer.WinStore.Pages.Settings;
using PresentationBus;

namespace Jukebox.WinStore.Modules
{
    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SettingsManager>()
                .As<ISettingsManager>()
                .SingleInstance()
                .OnActivated(x =>
                {
                    x.Instance.Add<SettingsController>("PlayerSettings", "Player Settings", c => c.PlayerSettings());
                    var bus = x.Context.Resolve<IPresentationBusConfiguration>();
                    bus.Subscribe(x.Instance);
                });

            builder
                .RegisterType<SettingsHandler>()
                .As<ISettingsHandler>()
                .SingleInstance();

        }
    }
}