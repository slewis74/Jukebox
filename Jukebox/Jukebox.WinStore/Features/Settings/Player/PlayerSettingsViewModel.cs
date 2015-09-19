using Jukebox.WinStore.Events;
using Orienteer.Data;
using PresentationBus;

namespace Jukebox.WinStore.Features.Settings.Player
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