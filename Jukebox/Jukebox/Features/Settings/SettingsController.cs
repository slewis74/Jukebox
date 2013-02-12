using System;
using Jukebox.Common;
using Jukebox.Features.Settings.Player;
using Jukebox.Requests;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Features.Settings
{
    public class SettingsController : JukeboxController
    {
        private readonly IPresentationBus _presentationBus;
        private readonly Func<bool, PlayerSettingsViewModel> _playerSettingsViewModelFactory;

        public SettingsController(
            IPresentationBus presentationBus,
            Func<bool, PlayerSettingsViewModel> playerSettingsViewModelFactory)
        {
            _presentationBus = presentationBus;
            _playerSettingsViewModelFactory = playerSettingsViewModelFactory;
        }

        public ActionResult PlayerSettings()
         {
             var request = new IsRandomPlayModeRequest();
             _presentationBus.Publish(request);
             var isRandomPlayMode = request.IsRandomPlayMode;

             return new SettingsPageActionResult<PlayerSettingsView, PlayerSettingsViewModel>(_playerSettingsViewModelFactory(isRandomPlayMode));
         }
    }
}