using System.Linq;
using Jukebox.Albums;
using Jukebox.Common;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;

namespace Jukebox.Artists
{
    public class ArtistController : JukeboxController
    {
        public ActionResult ShowAll(DistinctAsyncObservableCollection<Artist> artists)
        {
            return new PageActionResult(typeof(ArtistsView), new ArtistsViewModel(artists, HandlesPlaylists));
        }

        public ActionResult ShowArtist(Artist artist)
        {
            if (artist.Albums.Count == 1)
                return new PageActionResult(typeof(AlbumView), new AlbumViewModel(artist.Albums.Single(), HandlesPlaylists));
            return new PageActionResult(typeof(ArtistView), new ArtistViewModel(artist, HandlesPlaylists));
        }
    }
}