using Jukebox.Model;
using Slab.PresentationBus;

namespace Jukebox.Events
{
    public class CanMoveNextChangedEvent : PresentationEvent<Playlist>
    {
        public CanMoveNextChangedEvent(Playlist data, bool canMoveNext)
            : base(data)
        {
            CanMoveNext = canMoveNext;
        }

        public bool CanMoveNext { get; set; }
    }
}