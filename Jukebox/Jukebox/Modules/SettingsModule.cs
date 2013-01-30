using Autofac;
using Jukebox.Features.Settings;
using Jukebox.Storage;
using Slew.WinRT.Pages.Settings;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Modules
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
                                     var bus = x.Context.Resolve<IPresentationBus>();
                                     bus.Subscribe(x.Instance);
                                 });

            builder.RegisterType<SettingsHandler>().As<ISettingsHandler>().SingleInstance()
                                .OnActivated(x =>
                                {
                                    var bus = x.Context.Resolve<IPresentationBus>();
                                    bus.Subscribe(x.Instance);
                                });

        }
    }
}