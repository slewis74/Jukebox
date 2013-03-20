using System.Linq;
using Jukebox.Common;
using Jukebox.Features.Albums;
using Jukebox.Features.Artists.All;
using Jukebox.Features.Artists.Single;
using Jukebox.Storage;
using Slew.WinRT.Pages.Navigation;

namespace Jukebox.Features.Artists
{
    public class ArtistsController : JukeboxController
    {
        private readonly IMusicProvider _musicProvider;
        private readonly ArtistsViewModel.Factory _artistsViewModelFactory;
        private readonly AlbumViewModel.Factory _albumViewModelFactory;
        private readonly ArtistViewModel.Factory _artistViewModelFactory;

        public ArtistsController(
            IMusicProvider musicProvider,
            ArtistsViewModel.Factory artistsViewModelFactory,
            AlbumViewModel.Factory albumViewModelFactory,
            ArtistViewModel.Factory artistViewModelFactory)
        {
            _musicProvider = musicProvider;
            _artistsViewModelFactory = artistsViewModelFactory;
            _albumViewModelFactory = albumViewModelFactory;
            _artistViewModelFactory = artistViewModelFactory;
        }

        public ActionResult ShowAll()
        {
            return new ViewModelActionResult(() => _artistsViewModelFactory());
        }

        public ActionResult ShowArtist(string name)
        {
            var artist = _musicProvider.Artists.Single(a => a.Name == name);

            if (artist.Albums.Count == 1)
                return new ViewModelActionResult(() => _albumViewModelFactory(artist.Albums.Single()));
            return new ViewModelActionResult(() => _artistViewModelFactory(artist));
        }
    }
}