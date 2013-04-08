using Jukebox.Model;
using Slab.PresentationBus;

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