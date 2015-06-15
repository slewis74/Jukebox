using Jukebox.WinStore.Model;
using Orienteer.Data;

namespace Jukebox.WinStore.Features.Playlists
{
    public class PlaylistViewModel
    {
        private readonly Playlist _playlist;

        public PlaylistViewModel(Playlist playlist)
        {
            _playlist = playlist;
        }

        public DispatchingObservableCollection<PlaylistSong> Tracks { get { return _playlist; } }
    }
}