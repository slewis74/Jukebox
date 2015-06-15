using Jukebox.WinStore.Model;
using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;

namespace Jukebox.WinStore.Features.Artists.All
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