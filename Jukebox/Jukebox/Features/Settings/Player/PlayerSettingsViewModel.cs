using Jukebox.Events;
using Jukebox.Requests;
using Slew.WinRT.Data;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Features.Settings.Player
{
    public class PlayerSettingsViewModel : BindableBase
    {
        private readonly IPresentationBus _presentationBus;

        public PlayerSettingsViewModel(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
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

        public void Initialize()
        {
            var request = new IsRandomPlayModeRequest();
            _presentationBus.Publish(request);
            _isRandomPlayMode = request.IsRandomPlayMode;
        }
    }
}