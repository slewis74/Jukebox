using Orienteer.WinStore.Pages;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PlayDropLocationRequest : PresentationRequest
    {
        public Location Location { get; set; }
    }
}