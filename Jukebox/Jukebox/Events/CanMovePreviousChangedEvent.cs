using Jukebox.Model;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Events
{
    public class CanMovePreviousChangedEvent : PresentationEvent<Playlist>
    {
        public CanMovePreviousChangedEvent(Playlist data, bool canMovePrevious)
            : base(data)
        {
            CanMovePrevious = canMovePrevious;
        }

        public bool CanMovePrevious { get; set; }
    }
}