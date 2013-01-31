using System;
using System.Linq;
using Jukebox.Common;
using Jukebox.Features.Albums;
using Jukebox.Model;
using Jukebox.Storage;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;

namespace Jukebox.Features.Artists
{
    public class ArtistController : JukeboxController
    {
        private readonly IMusicProvider _musicProvider;
        private readonly Func<ArtistsViewModel> _artistsViewModelFactory;
        private readonly Func<Album, AlbumViewModel> _albumViewModelFactory;
        private readonly Func<Artist, ArtistViewModel> _artistViewModelFactory;

        public ArtistController(
            IMusicProvider musicProvider,
            Func<ArtistsViewModel> artistsViewModelFactory,
            Func<Album, AlbumViewModel> albumViewModelFactory,
            Func<Artist, ArtistViewModel> artistViewModelFactory)
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