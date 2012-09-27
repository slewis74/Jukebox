using System.Linq;
using Jukebox.Common;
using Jukebox.Features.Albums;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;

namespace Jukebox.Features.Artists
{
    public class ArtistController : JukeboxController
    {
        public ActionResult ShowAll(DistinctAsyncObservableCollection<Artist> artists)
        {
            return new PageActionResult(typeof(ArtistsView), Inject(() => new ArtistsViewModel(artists)));
        }

        public ActionResult ShowArtist(Artist artist)
        {
            if (artist.Albums.Count == 1)
                return new PageActionResult(typeof(AlbumView), Inject(() => new AlbumViewModel(artist.Albums.Single())));
            return new PageActionResult(typeof(ArtistView), Inject(() => new ArtistViewModel(artist)));
        }
    }
}