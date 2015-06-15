using System.Collections.Generic;
using Jukebox.WinStore.Model;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PlayNowRequest<T> : PresentationRequest
    {
        public PlayNowRequest(string artistName, Album album)
        {
            ArtistName = artistName;
            Album = album;
        }

        public string ArtistName { get; set; }
        public Album Album { get; set; }

        public T Scope { get; protected set; }
    }

    public class PlaySongNowRequest : PlayNowRequest<Song>
    {
        public PlaySongNowRequest(string artistName, Album album, Song song) : base(artistName, album)
        {
            Scope = song;
        }
    }
    public class PlayAlbumNowRequest : PlayNowRequest<Album>
    {
        public PlayAlbumNowRequest(string artistName, Album album) : base(artistName, album)
        {
            Scope = album;
        }
    }
    public class PlayArtistNowRequest : PlayNowRequest<Artist>
    {
        public PlayArtistNowRequest(Artist artist) : base(artist.Name, null)
        {
            Scope = artist;
        }
    }

    public class PlayAllNowRequest : PresentationRequest
    {
        public PlayAllNowRequest(IEnumerable<Artist> artists)
        {
            Artists = artists;
        }

        public IEnumerable<Artist> Artists { get; private set; }
    }
}