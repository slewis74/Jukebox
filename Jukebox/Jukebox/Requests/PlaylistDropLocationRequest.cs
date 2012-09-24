using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Requests
{
    public class PlaylistDropLocationRequest : PresentationRequest
    {
        public Location Location { get; set; }
    }
}