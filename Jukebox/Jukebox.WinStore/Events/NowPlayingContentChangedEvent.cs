using Jukebox.WinStore.Model;

namespace Jukebox.WinStore.Events
{
    public class NowPlayingContentChangedEvent : PlaylistContentChangedEvent
    {
        public NowPlayingContentChangedEvent(Playlist data) : base(data)
        {
        }
    }
}