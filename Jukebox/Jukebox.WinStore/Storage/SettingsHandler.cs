using Windows.Storage;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Requests;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Storage
{
    public class SettingsHandler :
        ISettingsHandler,
        IHandlePresentationRequest<IsRandomPlayModeRequest>,
        IHandlePresentationEvent<RandomPlayModeChangedEvent>
    {
        const string Settings = "Settings";
        const string RandomPlayMode = "IsRandomPlayMode";

        public bool IsGetRandomPlayMode()
        {
            var settingsContainer = ApplicationData.Current.LocalSettings.CreateContainer(Settings, ApplicationDataCreateDisposition.Always);

            if (settingsContainer.Values.Keys.Contains(RandomPlayMode))
            {
                return (bool)settingsContainer.Values[RandomPlayMode];
            }
            settingsContainer.Values[RandomPlayMode] = false;
            return false;
        }

        public void Handle(IsRandomPlayModeRequest request)
        {
            request.IsHandled = true;
            request.IsRandomPlayMode = IsGetRandomPlayMode();
        }

        public void Handle(RandomPlayModeChangedEvent presentationEvent)
        {
            var settingsContainer = ApplicationData.Current.LocalSettings.CreateContainer(Settings, ApplicationDataCreateDisposition.Always);
            settingsContainer.Values[RandomPlayMode] = presentationEvent.IsRandomPlayMode;
        }
    }
}