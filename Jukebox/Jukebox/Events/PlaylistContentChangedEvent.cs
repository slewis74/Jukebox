using Jukebox.Model;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Events
{
    public class PlaylistContentChangedEvent : PresentationEvent<Playlist>
    {
        public PlaylistContentChangedEvent(Playlist data, bool canMovePrevious, bool canMoveNext)
            : base(data)
        {
            CanMovePrevious = canMovePrevious;
            CanMoveNext = canMoveNext;
        }

        public bool CanMovePrevious { get; set; }
        public bool CanMoveNext { get; set; }
    }
}