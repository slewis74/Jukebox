using Jukebox.Features.Playlists;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class PlaylistsCommand : Command, ICanRequestNavigation
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