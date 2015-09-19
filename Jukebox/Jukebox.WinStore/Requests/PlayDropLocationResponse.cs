using Orienteer.WinStore.Pages;
using PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PlayDropLocationResponse : IPresentationResponse
    {
        public Location Location { get; set; }
    }
}