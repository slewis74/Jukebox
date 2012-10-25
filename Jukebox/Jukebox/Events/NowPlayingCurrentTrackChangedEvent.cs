using Jukebox.Model;

namespace Jukebox.Events
{
    public class NowPlayingCurrentTrackChangedEvent : PlaylistCurrentTrackChangedEvent
    {
        public NowPlayingCurrentTrackChangedEvent(Playlist data, Song song) : base(data, song)
        {
        }
    }
}