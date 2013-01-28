﻿using System.Linq;
using Jukebox.Common;
using Jukebox.Features.Albums;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;

namespace Jukebox.Features.Artists
{
    public class ArtistController : JukeboxController
    {
        public ActionResult ShowAll(DistinctAsyncObservableCollection<Artist> artists)
        {
            return new ViewModelActionResult(() => new ArtistsViewModel(PresentationBus, Navigator, artists));
        }

        public ActionResult ShowArtist(Artist artist)
        {
            if (artist.Albums.Count == 1)
                return new ViewModelActionResult(() => new AlbumViewModel(PresentationBus, Navigator, artist.Albums.Single()));
            return new ViewModelActionResult(() => new ArtistViewModel(PresentationBus, Navigator, artist));
        }
    }
}