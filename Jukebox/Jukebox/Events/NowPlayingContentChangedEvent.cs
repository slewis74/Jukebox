using Jukebox.Model;

namespace Jukebox.Events
{
    public class NowPlayingContentChangedEvent : PlaylistContentChangedEvent
    {
        public NowPlayingContentChangedEvent(Playlist data) : base(data)
        {
        }
    }
}