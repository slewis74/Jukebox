using Slew.WinRT.PresentationBus;
using Windows.Storage;

namespace Jukebox.Features.MainPage.Requests
{
    public class StopPlayingRequest : PresentationRequest
    {
    }

    public class RestartPlayingRequest : PresentationRequest
    {
    }

    public class PausePlayingRequest : PresentationRequest
    {
    }

    public class PlayFileRequest : PresentationRequest
    {
        public PlayFileRequest(StorageFile storageFile)
        {
            StorageFile = storageFile;
        }

        public StorageFile StorageFile { get; set; }
    }
}