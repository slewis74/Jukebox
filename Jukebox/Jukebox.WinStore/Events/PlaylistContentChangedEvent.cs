using Jukebox.WinStore.Model;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class PlaylistContentChangedEvent : PresentationEvent
    {
        public PlaylistContentChangedEvent(Playlist playlist)
        {
            Playlist = playlist;
        }

        public Playlist Playlist { get; set; }
    }
}