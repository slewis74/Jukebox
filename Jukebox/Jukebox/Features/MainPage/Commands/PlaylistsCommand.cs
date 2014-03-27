using Jukebox.Features.Playlists;
using Jukebox.Model;
using Slab.Data;
using Slab.Pages.Navigation;
using Slab.Xaml.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class PlaylistsCommand : Command
    {
        private readonly DistinctAsyncObservableCollection<Playlist> _playlists;

        public PlaylistsCommand(DistinctAsyncObservableCollection<Playlist> playlists)
        {
            _playlists = playlists;
        }

        public INavigator Navigator { get; set; }

        public override void Execute(object parameter)
        {
            Navigator.Navigate<PlaylistController>(c => c.ShowAll(_playlists));
        }
    }
}