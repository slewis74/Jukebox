using Jukebox.WinStore.Model;

namespace Jukebox.WinStore.Events
{
    public class NowPlayingCurrentTrackChangedEvent : PlaylistCurrentTrackChangedEvent
    {
        public NowPlayingCurrentTrackChangedEvent(Playlist data, PlaylistSong playlistSong) : base(data, playlistSong)
        {
        }
    }
}