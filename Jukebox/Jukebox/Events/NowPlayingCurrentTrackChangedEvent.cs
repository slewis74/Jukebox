using Jukebox.Model;

namespace Jukebox.Events
{
    public class NowPlayingCurrentTrackChangedEvent : PlaylistCurrentTrackChangedEvent
    {
        public NowPlayingCurrentTrackChangedEvent(Playlist data, Song song, bool canMovePrevious, bool canMoveNext) : base(data, song, canMovePrevious, canMoveNext)
        {
        }
    }
}