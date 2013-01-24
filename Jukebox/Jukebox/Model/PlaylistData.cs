using System.Collections.Generic;
using System.Linq;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Model
{
    public class PlaylistData
    {
        public PlaylistData(IPresentationBus presentationBus, bool isRandomPlayMode, IEnumerable<Song> nowPlayingSongs, int? currentTrackIndex)
        {
            NowPlayingPlaylist =
                new NowPlayingPlaylist(presentationBus, isRandomPlayMode, nowPlayingSongs, currentTrackIndex);

            Playlists = Enumerable.Empty<Playlist>();
        }

        public NowPlayingPlaylist NowPlayingPlaylist { get; private set; }
        public IEnumerable<Playlist> Playlists { get; set; }
    }
}