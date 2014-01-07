using System;
using System.Threading.Tasks;
using Jukebox.Common;
using Jukebox.Features.Settings.Player;
using Jukebox.Requests;
using Slab.Pages.Navigation;
using Slab.PresentationBus;
using SlabRt.Pages.Navigation;

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

        public async Task<ActionResult> PlayerSettings()
         {
             var request = new IsRandomPlayModeRequest();
             await _presentationBus.PublishAsync(request);
             var isRandomPlayMode = request.IsRandomPlayMode;

             return new SettingsPageActionResult<PlayerSettingsView, PlayerSettingsViewModel>(_playerSettingsViewModelFactory(isRandomPlayMode));
         }
    }
}