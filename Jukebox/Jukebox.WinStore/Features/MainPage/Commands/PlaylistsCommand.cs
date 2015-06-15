using Jukebox.WinStore.Features.Playlists;
using Jukebox.WinStore.Model;
using Orienteer.Data;
using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;

namespace Jukebox.WinStore.Features.MainPage.Commands
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