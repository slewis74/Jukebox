using Windows.Storage;
using PresentationBus;

namespace Jukebox.WinStore.Features.MainPage.Requests
{
    public class StopPlayingCommand : IPresentationCommand
    {
    }

    public class RestartPlayingCommand : IPresentationCommand
    {
    }

    public class PausePlayingCommand : IPresentationCommand
    {
    }

    public class PlayFileCommand : IPresentationCommand
    {
        public PlayFileCommand(string artistName, string trackTitle, StorageFile storageFile)
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