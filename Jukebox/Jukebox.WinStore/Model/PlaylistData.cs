using System.Collections.Generic;
using System.Linq;
using PresentationBus;

namespace Jukebox.WinStore.Model
{
    public class PlaylistData
    {
        public delegate PlaylistData Factory(bool isRandomPlayMode, IEnumerable<PlaylistSong> nowPlayingSongs, int? currentTrackIndex);

        public PlaylistData(NowPlayingPlaylist.WithTracksFactory nowPlayingFactory, bool isRandomPlayMode, IEnumerable<PlaylistSong> nowPlayingSongs, int? currentTrackIndex)
        {
            NowPlayingPlaylist =
                nowPlayingFactory(isRandomPlayMode, nowPlayingSongs, currentTrackIndex);

            Playlists = Enumerable.Empty<Playlist>();
        }

        public NowPlayingPlaylist NowPlayingPlaylist { get; private set; }
        public IEnumerable<Playlist> Playlists { get; set; }
    }
}