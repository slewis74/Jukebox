using System;
using System.Linq;
using Jukebox.WinStore.Common;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Storage;
using Orienteer.Pages.Navigation;

namespace Jukebox.WinStore.Features.Albums
{
    public class AlbumController : JukeboxController
    {
        private readonly IMusicProvider _musicProvider;
        private readonly AlbumViewModel.Factory _albumViewModelFactory;

        public AlbumController(
            IMusicProvider musicProvider,
            AlbumViewModel.Factory albumViewModelFactory)
        {
            _musicProvider = musicProvider;
            _albumViewModelFactory = albumViewModelFactory;
        }

        public ActionResult ShowAlbum(string artistName, string albumTitle)
         {
             var artist = _musicProvider.Artists.Single(a => a.Name == artistName);

             return new ViewModelActionResult(() => _albumViewModelFactory(artist, artist.Albums.Single(a => a.Title == albumTitle)));
         }
    }
}