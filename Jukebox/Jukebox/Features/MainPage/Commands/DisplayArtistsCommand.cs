using Jukebox.Features.Artists;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class DisplayArtistsCommand : Command, ICanRequestNavigation
    {
        private readonly DistinctAsyncObservableCollection<Artist> _artists;

        public DisplayArtistsCommand(DistinctAsyncObservableCollection<Artist> artists)
        {
            _artists = artists;
        }

        public INavigator Navigator { get; set; }

        public override void Execute(object parameter)
        {
            Navigator.Navigate<ArtistController>(c => c.ShowAll(_artists));
        }
    }
}