using Jukebox.WinStore.Model;
using PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class AddToCurrentPlaylistCommand : IPresentationCommand
    {
    }

    public class AddSongToCurrentPlaylistCommand : AddToCurrentPlaylistCommand
    {
        public Song Song { get; set; }
    }
    public class AddAlbumToCurrentPlaylistCommand : AddToCurrentPlaylistCommand
    {
        public Album Album { get; set; }
    }
    public class AddArtistToCurrentPlaylistCommand : AddToCurrentPlaylistCommand
    {
        public Artist Artist { get; set; }
    }
}