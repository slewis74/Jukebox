using System.Collections.Generic;
using Jukebox.WinStore.Model;
using PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class AlbumDataLoaded : PresentationEvent
    {
        public AlbumDataLoaded(IEnumerable<Artist> artists)
        {
            Artists = artists;
        }

        public IEnumerable<Artist> Artists { get; set; }
    }
}