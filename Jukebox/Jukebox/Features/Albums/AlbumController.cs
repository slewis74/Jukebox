using Jukebox.Common;
using Jukebox.Model;
using Slew.WinRT.Pages.Navigation;

namespace Jukebox.Features.Albums
{
    public class AlbumController : JukeboxController
    {
         public ActionResult ShowAlbum(Album album)
         {
             return new ViewModelActionResult(() => new AlbumViewModel(PresentationBus, Navigator, album));
         }
    }
}