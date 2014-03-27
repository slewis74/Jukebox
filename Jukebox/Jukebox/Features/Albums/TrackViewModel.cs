using System;
using Jukebox.Model;
using Slab.Data;
using Slab.WinStore.Pages;

namespace Jukebox.Features.Albums
{
    public class TrackViewModel
    {
        private Song _song;

        public TrackViewModel(Song song, AsyncObservableCollection<LocationCommandMapping> trackLocationCommandMappings)
        {
            _song = song;
            TrackLocationCommandMappings = trackLocationCommandMappings;
        }

        public AsyncObservableCollection<LocationCommandMapping> TrackLocationCommandMappings { get; private set; }

        public uint TrackNumber { get { return _song.TrackNumber; } }
        public string Title { get { return _song.Title; } }
        public uint DiscNumber { get { return _song.DiscNumber; } }
        public TimeSpan Duration { get { return _song.Duration; } }

        public Song GetSong()
        {
            return _song;
        }
    }
}