using Jukebox.Model;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Events
{
    public class PlaylistCurrentTrackChangedEvent : PresentationEvent<Playlist>
    {
        public PlaylistCurrentTrackChangedEvent(Playlist data, Song song, bool canMovePrevious, bool canMoveNext)
            : base(data)
        {
            Song = song;
            CanMovePrevious = canMovePrevious;
            CanMoveNext = canMoveNext;
        }

        public Song Song { get; set; }
        public bool CanMovePrevious { get; set; }
        public bool CanMoveNext { get; set; }
    }
}