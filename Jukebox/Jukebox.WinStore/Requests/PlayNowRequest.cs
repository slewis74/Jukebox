using System.Collections.Generic;
using Jukebox.WinStore.Model;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PlayNowRequest<T> : PresentationRequest
    {
        public PlayNowRequest(string artistName, string albumTitle)
        {
            ArtistName = artistName;
            AlbumTitle = albumTitle;
        }

        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }

        public T Scope { get; protected set; }
    }

    public class PlaySongNowRequest : PlayNowRequest<Song>
    {
        public PlaySongNowRequest(string artistName, string albumTitle, Song song) : base(artistName, albumTitle)
        {
            Scope = song;
        }
    }
    public class PlayAlbumNowRequest : PlayNowRequest<Album>
    {
        public PlayAlbumNowRequest(string artistName, Album album) : base(artistName, album.Title)
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