using Jukebox.Features.Artists;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class DisplayArtistsCommand : Command
    {
        private readonly INavigator _navigator;

        public DisplayArtistsCommand(INavigator navigator)
        {
            _navigator = navigator;
        }

        public override void Execute(object parameter)
        {
            _navigator.Navigate<ArtistController>(c => c.ShowAll());
        }
    }
}