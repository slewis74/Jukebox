using Jukebox.WinStore.Model;

namespace Jukebox.WinStore.Events
{
    public class NowPlayingCurrentTrackChangedEvent : PlaylistCurrentTrackChangedEvent
    {
        public NowPlayingCurrentTrackChangedEvent(Playlist data, Song song) : base(data, song)
        {
        }
    }
}