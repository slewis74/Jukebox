using Jukebox.WinStore.Model;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class CanMoveNextChangedEvent : PresentationEvent
    {
        public CanMoveNextChangedEvent(Playlist playlist, bool canMoveNext)
        {
            Playlist = playlist;
            CanMoveNext = canMoveNext;
        }

        public Playlist Playlist { get; set; }
        public bool CanMoveNext { get; set; }
    }
}