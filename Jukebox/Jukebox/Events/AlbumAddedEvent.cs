using Jukebox.Model;
using Slab.PresentationBus;

namespace Jukebox.Events
{
    public class AlbumAddedEvent : PresentationEvent
    {
        public AlbumAddedEvent(Artist artist, Album album)
        {
            Artist = artist;
            Album = album;
        }

        public Artist Artist { get; set; }
        public Album Album { get; set; }
    }
}