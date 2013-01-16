using System.Collections.Generic;
using System.Linq;
using Slew.WinRT.Container;

namespace Jukebox.Model
{
    public class PlaylistData
    {
        public PlaylistData(bool isRandomPlayMode, IEnumerable<Song> nowPlayingSongs, int? currentTrackIndex)
        {
            NowPlayingPlaylist =
                new NowPlayingPlaylist(isRandomPlayMode, nowPlayingSongs, currentTrackIndex);

            Playlists = Enumerable.Empty<Playlist>();
        }

        public NowPlayingPlaylist NowPlayingPlaylist { get; private set; }
        public IEnumerable<Playlist> Playlists { get; set; }
    }
}