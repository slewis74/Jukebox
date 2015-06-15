using Jukebox.WinStore.Common;
using Jukebox.WinStore.Model;
using Orienteer.Data;
using Orienteer.Pages.Navigation;

namespace Jukebox.WinStore.Features.Playlists
{
    public class PlaylistController : JukeboxController
    {
         public ActionResult ShowAll(DistinctAsyncObservableCollection<Playlist> playlists)
         {
             return new ViewModelActionResult(() => new PlaylistsViewModel(playlists));
         }
    }
}