using System;
using Jukebox.WinStore.Model;
using Slab.Data;
using Slab.WinStore.Pages;

namespace Jukebox.WinStore.Features.Albums
{
    public class TrackViewModel
    {
        private Song _song;

        public TrackViewModel(string artistName, string albumTitle, Song song, AsyncObservableCollection<LocationCommandMapping> trackLocationCommandMappings)
        {
            _song = song;
            ArtistName = artistName;
            AlbumTitle = albumTitle;
            TrackLocationCommandMappings = trackLocationCommandMappings;
        }

        public AsyncObservableCollection<LocationCommandMapping> TrackLocationCommandMappings { get; private set; }

        public string ArtistName { get; private set; }
        public string AlbumTitle { get; private set; }

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