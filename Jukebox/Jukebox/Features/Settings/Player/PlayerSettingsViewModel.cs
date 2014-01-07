using Jukebox.Events;
using Slab.Data;
using Slab.PresentationBus;

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
                    _presentationBus.PublishAsync(new RandomPlayModeChangedEvent(_isRandomPlayMode));
                }
            }
        }
    }
}