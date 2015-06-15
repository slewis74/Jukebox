using System;
using Jukebox.WinStore.Model;
using Orienteer.Data;
using Orienteer.WinStore.Pages;

namespace Jukebox.WinStore.Features.Albums
{
    public class TrackViewModel
    {
        private readonly Song _song;

        public TrackViewModel(string artistName, Album album, Song song, DispatchingObservableCollection<LocationCommandMapping> trackLocationCommandMappings)
        {
            _song = song;
            ArtistName = artistName;
            Album = album;
            TrackLocationCommandMappings = trackLocationCommandMappings;
        }

        public DispatchingObservableCollection<LocationCommandMapping> TrackLocationCommandMappings { get; private set; }

        public string ArtistName { get; private set; }
        public Album Album { get; private set; }
        public string AlbumTitle { get { return Album.Title; } }

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