using System.Threading.Tasks;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Storage;
using Slew.PresentationBus;

namespace Jukebox.WinStore.EventHandlers.WhenASong.GetsLoaded
{
    public class LoadBitmapsForAlbum : 
        OnDemandEventHandler,
        IHandlePresentationEventAsync<SongLoadedEvent>
    {
        private readonly IAlbumArtStorage _albumArtStorage;

        public LoadBitmapsForAlbum(IAlbumArtStorage albumArtStorage)
        {
            _albumArtStorage = albumArtStorage;
        }

        public async Task HandleAsync(SongLoadedEvent fact)
        {
            if (string.IsNullOrWhiteSpace(fact.Album.SmallBitmapUri) == false)
                return;

            if ((await _albumArtStorage.AlbumFolderExists(fact.Album.Folder)) == false)
            {
                // 200 pixel is used by the pages, others are used for tiles
                await _albumArtStorage.SaveBitmapAsync(fact.Album.Folder, 150, fact.Song.Path);
                await _albumArtStorage.SaveBitmapAsync(fact.Album.Folder, 200, fact.Song.Path);
                await _albumArtStorage.SaveBitmapAsync(fact.Album.Folder, 310, fact.Song.Path);
            }

            fact.Album.SmallBitmapUri = "ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(fact.Album.Folder, 200).Replace(@"\", "/");
            fact.Album.LargeBitmapUri = "ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(fact.Album.Folder, 310).Replace(@"\", "/");
        }
    }
}