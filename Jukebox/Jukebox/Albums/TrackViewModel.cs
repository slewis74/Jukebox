using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;

namespace Jukebox.Albums
{
    public class TrackViewModel
    {
        public TrackViewModel(Song song, AsyncObservableCollection<LocationCommandMapping> trackLocationCommandMappings)
        {
            Song = song;
            TrackLocationCommandMappings = trackLocationCommandMappings;
        }

        public Song Song { get; set; }
        public AsyncObservableCollection<LocationCommandMapping> TrackLocationCommandMappings { get; private set; }
    }
}