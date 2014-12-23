using Jukebox.WinStore.Model;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class PlaylistCurrentTrackChangedEvent : PresentationEvent<Playlist>
    {
        public PlaylistCurrentTrackChangedEvent(Playlist data, PlaylistSong playlistSong)
            : base(data)
        {
            PlaylistSong = playlistSong;
        }

        public PlaylistSong PlaylistSong { get; set; }
    }
}