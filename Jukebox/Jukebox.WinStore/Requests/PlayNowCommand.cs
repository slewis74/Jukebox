using System.Collections.Generic;
using Jukebox.WinStore.Model;
using PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PlayNowCommand<T> : IPresentationCommand
    {
        public PlayNowCommand(string artistName, Album album)
        {
            ArtistName = artistName;
            Album = album;
        }

        public string ArtistName { get; set; }
        public Album Album { get; set; }

        public T Scope { get; protected set; }
    }

    public class PlaySongNowCommand : PlayNowCommand<Song>
    {
        public PlaySongNowCommand(string artistName, Album album, Song song) : base(artistName, album)
        {
            Scope = song;
        }
    }
    public class PlayAlbumNowCommand : PlayNowCommand<Album>
    {
        public PlayAlbumNowCommand(string artistName, Album album) : base(artistName, album)
        {
            Scope = album;
        }
    }
    public class PlayArtistNowCommand : PlayNowCommand<Artist>
    {
        public PlayArtistNowCommand(Artist artist) : base(artist.Name, null)
        {
            Scope = artist;
        }
    }

    public class PlayAllNowCommand : IPresentationCommand
    {
        public PlayAllNowCommand(IEnumerable<Artist> artists)
        {
            Artists = artists;
        }

        public IEnumerable<Artist> Artists { get; private set; }
    }
}