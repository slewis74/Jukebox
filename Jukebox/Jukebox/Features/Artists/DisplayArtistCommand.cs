using Jukebox.Model;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.Artists
{
    public class DisplayArtistCommand : NavigationCommand<Artist>
    {
        public DisplayArtistCommand(INavigator navigator) : base(navigator)
        {}

        public override void Execute(Artist parameter)
        {
            Navigator.Navigate<ArtistController>(c => c.ShowArtist(parameter));
        }
    }
}