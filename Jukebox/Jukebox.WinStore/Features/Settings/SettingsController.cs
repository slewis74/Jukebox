using System;
using System.Threading.Tasks;
using Jukebox.WinStore.Common;
using Jukebox.WinStore.Features.Settings.Player;
using Jukebox.WinStore.Requests;
using Orienteer.Pages.Navigation;
using Orienteer.WinStore.Pages.Navigation;
using PresentationBus;

namespace Jukebox.WinStore.Features.Settings
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

        public async Task<ActionResult> PlayerSettings()
         {
             var request = new IsRandomPlayModeRequest();
             var response = await _presentationBus.Request(request);
             var isRandomPlayMode = response.IsRandomPlayMode;

             return new SettingsPageActionResult<PlayerSettingsView, PlayerSettingsViewModel>(_playerSettingsViewModelFactory(isRandomPlayMode));
         }
    }
}