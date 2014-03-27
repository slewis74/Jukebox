using Jukebox.Model;
using Slab.Pages.Navigation;
using Slab.Xaml.ViewModels;

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