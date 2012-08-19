using Jukebox.Common;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;

namespace Jukebox.Playlists
{
    public class PlaylistController : JukeboxController
    {
         public ActionResult ShowAll(DistinctAsyncObservableCollection<Playlist> playlists)
         {
             return new PageActionResult(typeof(PlaylistsView), new PlaylistsViewModel(playlists));
         }
    }
}