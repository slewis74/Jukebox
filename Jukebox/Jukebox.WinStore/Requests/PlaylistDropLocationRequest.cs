using Orienteer.WinStore.Pages;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PlaylistDropLocationRequest : PresentationRequest
    {
        public Location Location { get; set; }
    }
}