using Jukebox.WinStore.Model;
using PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class CanMovePreviousChangedEvent : PresentationEvent
    {
        public CanMovePreviousChangedEvent(Playlist playlist, bool canMovePrevious)
        {
            Playlist = playlist;
            CanMovePrevious = canMovePrevious;
        }

        public Playlist Playlist { get; set; }
        public bool CanMovePrevious { get; set; }
    }
}