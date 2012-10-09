using System.Linq;
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
            return new PageActionResult<ArtistsView>(Inject(() => new ArtistsViewModel(artists)));
        }

        public ActionResult ShowArtist(Artist artist)
        {
            if (artist.Albums.Count == 1)
                return new PageActionResult<AlbumView>(Inject(() => new AlbumViewModel(artist.Albums.Single())));
            return new PageActionResult<ArtistView>(Inject(() => new ArtistViewModel(artist)));
        }
    }
}