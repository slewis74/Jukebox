using Jukebox.Model;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.Artists.All
{
    public class DisplayArtistCommand : NavigationCommand<Artist>
    {
        public DisplayArtistCommand(INavigator navigator) : base(navigator)
        {}

        public override void Execute(Artist parameter)
        {
            Navigator.Navigate<ArtistsController>(c => c.ShowArtist(parameter.Name));
        }
    }
}