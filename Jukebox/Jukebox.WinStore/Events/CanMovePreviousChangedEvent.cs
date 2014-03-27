using Jukebox.WinStore.Model;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Events
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