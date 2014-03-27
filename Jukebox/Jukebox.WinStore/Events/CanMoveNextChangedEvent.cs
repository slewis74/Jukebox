using Jukebox.WinStore.Model;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Events
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