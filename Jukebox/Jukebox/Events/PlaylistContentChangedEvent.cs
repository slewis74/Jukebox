using Jukebox.Model;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Events
{
    public class PlaylistContentChangedEvent : PresentationEvent<Playlist>
    {
        public PlaylistContentChangedEvent(Playlist data)
            : base(data)
        {
        }
    }
}