using Jukebox.Common;
using Jukebox.Features.Settings.Player;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Features.Settings
{
    public class SettingsController : JukeboxController
    {
        public IPresentationBus PresentationBus { get; set; }

        public ActionResult PlayerSettings()
         {
             return new SettingsPageActionResult<PlayerSettingsView, PlayerSettingsViewModel>(new PlayerSettingsViewModel(PresentationBus));
         }
    }
}