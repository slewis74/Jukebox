using Jukebox.WinStore.Model;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class AddToCurrentPlaylistRequest : PresentationRequest
    {
    }

    public class AddSongToCurrentPlaylistRequest : AddToCurrentPlaylistRequest
    {
        public Song Song { get; set; }
    }
    public class AddAlbumToCurrentPlaylistRequest : AddToCurrentPlaylistRequest
    {
        public Album Album { get; set; }
    }
    public class AddArtistToCurrentPlaylistRequest : AddToCurrentPlaylistRequest
    {
        public Artist Artist { get; set; }
    }
}