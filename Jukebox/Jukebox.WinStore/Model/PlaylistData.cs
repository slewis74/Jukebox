using System.Collections.Generic;
using System.Linq;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Model
{
    public class PlaylistData
    {
        public PlaylistData(IPresentationBus presentationBus, bool isRandomPlayMode, IEnumerable<PlaylistSong> nowPlayingSongs, int? currentTrackIndex)
        {
            NowPlayingPlaylist =
                new NowPlayingPlaylist(presentationBus, isRandomPlayMode, nowPlayingSongs, currentTrackIndex);

            Playlists = Enumerable.Empty<Playlist>();
        }

        public NowPlayingPlaylist NowPlayingPlaylist { get; private set; }
        public IEnumerable<Playlist> Playlists { get; set; }
    }
}