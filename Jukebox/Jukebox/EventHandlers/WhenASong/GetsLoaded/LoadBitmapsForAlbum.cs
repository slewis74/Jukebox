using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Jukebox.Events;
using Jukebox.Model;
using Jukebox.Storage;
using Slab.PresentationBus;

namespace Jukebox.EventHandlers.WhenASong.GetsLoaded
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

            if ((await _albumArtStorage.AlbumFolderExists(fact.Album.Artist.Name, fact.Album.Title)) == false)
            {
                // 200 pixel is used by the pages, others are used for tiles
                await _albumArtStorage.SaveBitmapAsync(fact.Album.Artist.Name, fact.Album.Title, 150, fact.Song.Path);
                await _albumArtStorage.SaveBitmapAsync(fact.Album.Artist.Name, fact.Album.Title, 200, fact.Song.Path);
                await _albumArtStorage.SaveBitmapAsync(fact.Album.Artist.Name, fact.Album.Title, 310, fact.Song.Path);
            }

            fact.Album.SmallBitmapUri = "ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(fact.Album.Artist.Name, fact.Album.Title, 200).Replace(@"\", "/");
            fact.Album.LargeBitmapUri = "ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(fact.Album.Artist.Name, fact.Album.Title, 310).Replace(@"\", "/");
        }
    }
}