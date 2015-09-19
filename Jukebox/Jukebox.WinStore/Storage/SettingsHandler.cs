using Windows.Storage;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Requests;
using PresentationBus;

namespace Jukebox.WinStore.Storage
{
    public class SettingsHandler :
        ISettingsHandler,
        IHandlePresentationRequest<IsRandomPlayModeRequest, IsRandomPlaymodeResponse>,
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

        public IsRandomPlaymodeResponse Handle(IsRandomPlayModeRequest request)
        {
            return new IsRandomPlaymodeResponse { IsRandomPlayMode = IsGetRandomPlayMode()} ;
        }

        public void Handle(RandomPlayModeChangedEvent presentationEvent)
        {
            var settingsContainer = ApplicationData.Current.LocalSettings.CreateContainer(Settings, ApplicationDataCreateDisposition.Always);
            settingsContainer.Values[RandomPlayMode] = presentationEvent.IsRandomPlayMode;
        }
    }
}