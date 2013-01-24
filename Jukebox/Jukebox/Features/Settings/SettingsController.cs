using Jukebox.Common;
using Jukebox.Features.Settings.Player;
using Slew.WinRT.Pages.Navigation;

namespace Jukebox.Features.Settings
{
    public class SettingsController : JukeboxController
    {
         public ActionResult PlayerSettings()
         {
             return new SettingsPageActionResult<PlayerSettingsView, PlayerSettingsViewModel>(new PlayerSettingsViewModel(null));
         }
    }
}