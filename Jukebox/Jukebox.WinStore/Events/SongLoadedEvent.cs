using Jukebox.WinStore.Model;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class SongLoadedEvent : PresentationEvent
    {
        public SongLoadedEvent(Album album, Song song)
        {
            Album = album;
            Song = song;
        }

        public Album Album { get; private set; }
        public Song Song { get; private set; }
    }
}