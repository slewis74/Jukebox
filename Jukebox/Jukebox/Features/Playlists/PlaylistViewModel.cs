using Jukebox.Model;
using Slab.Data;

namespace Jukebox.Features.Playlists
{
    public class PlaylistViewModel
    {
        private readonly Playlist _playlist;

        public PlaylistViewModel(Playlist playlist)
        {
            _playlist = playlist;
        }

        public AsyncObservableCollection<Song> Tracks { get { return _playlist; } }
    }
}