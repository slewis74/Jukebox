using Jukebox.Common;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;

namespace Jukebox.Features.Playlists
{
    public class PlaylistController : JukeboxController
    {
         public ActionResult ShowAll(DistinctAsyncObservableCollection<Playlist> playlists)
         {
             return new ViewModelActionResult(() => new PlaylistsViewModel(playlists));
         }
    }
}