using Jukebox.Requests;
using Slew.WinRT.Container;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.Artists
{
    public class PlayAllCommand : Command, IPublish
    {
        public PlayAllCommand()
        {
            PropertyInjector.Inject(() => this);
        }

        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(object parameter)
        {
            PresentationBus.Publish(new PlayAllNowRequest());
        }
    }
}