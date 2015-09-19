using Jukebox.WinStore.Model;
using PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class PlaylistCurrentTrackChangedEvent : PresentationEvent
    {
        public PlaylistCurrentTrackChangedEvent(Playlist playlist, PlaylistSong playlistSong)
        {
            Playlist = playlist;
            PlaylistSong = playlistSong;
        }

        public Playlist Playlist { get; set; }
        public PlaylistSong PlaylistSong { get; set; }
    }
}