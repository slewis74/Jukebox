using Jukebox.Common;
using Jukebox.Model;
using Slew.WinRT.Pages;

namespace Jukebox.Features.Albums
{
    public class AlbumController : JukeboxController
    {
         public ActionResult ShowAlbum(Album album)
         {
             return new PageActionResult(typeof(AlbumView), Resolve(()=> new AlbumViewModel(album)));
         }
    }
}