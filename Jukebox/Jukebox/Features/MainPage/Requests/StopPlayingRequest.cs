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
        public PlayFileRequest(string artistName, string trackTitle, StorageFile storageFile)
        {
            ArtistName = artistName;
            TrackTitle = trackTitle;
            StorageFile = storageFile;
        }

        public string ArtistName { get; set; }
        public string TrackTitle { get; set; }
        public StorageFile StorageFile { get; set; }
    }
}