﻿using System.Linq;
using Jukebox.WinStore.Common;
using Jukebox.WinStore.Features.Albums;
using Jukebox.WinStore.Features.Artists.All;
using Jukebox.WinStore.Features.Artists.Single;
using Jukebox.WinStore.Storage;
using Orienteer.Pages.Navigation;

namespace Jukebox.WinStore.Features.Artists
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
            var artist = _musicProvider.Artists.SingleOrDefault(a => a.Name == name);

            if (artist == null)
                return ShowAll();
            if (artist.Albums.Count == 1)
                return new ViewModelActionResult(() => _albumViewModelFactory(artist, artist.Albums.Single()));
            return new ViewModelActionResult(() => _artistViewModelFactory(artist));
        }
    }
}