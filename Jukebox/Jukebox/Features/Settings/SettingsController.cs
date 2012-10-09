using Jukebox.Common;
using Jukebox.Features.Settings.Player;
using Slew.WinRT.Pages;

namespace Jukebox.Features.Settings
{
    public class SettingsController : JukeboxController
    {
         public ActionResult PlaySettings()
         {
             return new SettingsPageActionResult<PlayerSettingsView>(Inject(() => new PlayerSettingsViewModel()));
         }
    }
}