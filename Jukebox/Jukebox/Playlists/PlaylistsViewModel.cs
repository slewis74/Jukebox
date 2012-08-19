using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.ViewModels;

namespace Jukebox.Playlists
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