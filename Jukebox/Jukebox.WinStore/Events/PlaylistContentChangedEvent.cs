using Jukebox.WinStore.Model;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class PlaylistContentChangedEvent : PresentationEvent<Playlist>
    {
        public PlaylistContentChangedEvent(Playlist data)
            : base(data)
        {
        }
    }
}