using Jukebox.Model;

namespace Jukebox.Events
{
    public class NowPlayingContentChangedEvent : PlaylistContentChangedEvent
    {
        public NowPlayingContentChangedEvent(
            Playlist data, 
            bool canMovePrevious, 
            bool canMoveNext) : base(data)
        {
            CanMovePrevious = canMovePrevious;
            CanMoveNext = canMoveNext;
        }

        public bool CanMovePrevious { get; set; }
        public bool CanMoveNext { get; set; }
    }
}