using Orienteer.WinStore.Pages;
using PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PlaylistDropLocationRequest : PresentationRequest<PlaylistDropLocationRequest, PlaylistDropLocationResponse>
    {
    }

    public class PlaylistDropLocationResponse : IPresentationResponse
    {
        public Location Location { get; set; }
    }
}