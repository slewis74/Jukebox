using Jukebox.Events;
using Slew.WinRT.Data;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Features.Settings.Player
{
    public class PlayerSettingsViewModel : BindableBase
    {
        private readonly IPresentationBus _presentationBus;

        public PlayerSettingsViewModel(IPresentationBus presentationBus, bool isRandomPlayMode)
        {
            _presentationBus = presentationBus;
            _isRandomPlayMode = isRandomPlayMode;
        }

        private bool _isRandomPlayMode;
        public bool IsRandomPlayMode
        {
            get { return _isRandomPlayMode; }
            set
            {
                if (SetProperty(ref _isRandomPlayMode, value))
                {
                    _presentationBus.Publish(new RandomPlayModeChangedEvent(_isRandomPlayMode));
                }
            }
        }
    }
}