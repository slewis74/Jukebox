using Jukebox.Features.Artists;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class DisplayArtistsCommand : Command
    {
        private readonly INavigator _navigator;
        private readonly DistinctAsyncObservableCollection<Artist> _artists;

        public DisplayArtistsCommand(INavigator navigator, DistinctAsyncObservableCollection<Artist> artists)
        {
            _navigator = navigator;
            _artists = artists;
        }

        public override void Execute(object parameter)
        {
            _navigator.Navigate<ArtistController>(c => c.ShowAll(_artists));
        }
    }
}