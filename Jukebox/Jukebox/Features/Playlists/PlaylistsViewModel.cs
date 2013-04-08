using Jukebox.Model;
using Slab.Data;
using Slab.ViewModels;

namespace Jukebox.Features.Playlists
{
    public class PlaylistsViewModel
    {
        public PlaylistsViewModel(DistinctAsyncObservableCollection<Playlist> playlists)
        {
            Playlists = playlists;
        }

        public DistinctAsyncObservableCollection<Playlist> Playlists { get; set; }
    }

    public class AddPlaylistCommand : Command
    {
        public override void Execute(object parameter)
        {
        }
    }

    public class DeletePlaylistCommand : Command<Playlist>
    {
        public override void Execute(Playlist parameter)
        {
            
        }
    }
}