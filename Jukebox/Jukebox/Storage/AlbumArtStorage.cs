using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Storage
{
    public class AlbumArtStorage : IAlbumArtStorage
    {
        public async Task SaveBitmapAsync(string artist, string album, uint size, string songPath)
        {
            var songFile = await StorageFile.GetFileFromPathAsync(songPath);

            using (var thumbnail = await songFile.GetThumbnailAsync(ThumbnailMode.MusicView, size) ??
                                   await songFile.GetThumbnailAsync(ThumbnailMode.VideosView, size))
            {
                if (thumbnail == null)
                    return;

                var reader = new DataReader(thumbnail);
                var fileLength = (uint)thumbnail.Size;
                await reader.LoadAsync(fileLength);

                var buffer = reader.ReadBuffer(fileLength);

                var memStream = new InMemoryRandomAccessStream();

                await memStream.WriteAsync(buffer);
                await memStream.FlushAsync();
                memStream.Seek(0);

                await ApplicationData.Current.LocalFolder.CreateFolderAsync(AlbumArtFolderName(artist, album), CreationCollisionOption.OpenIfExists);

                var albumArtFileName = AlbumArtFileName(artist, album, size);
                var outputFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(albumArtFileName, CreationCollisionOption.ReplaceExisting);
                using (var outputStream = await outputFile.OpenStreamForWriteAsync())
                {
                    await outputStream.WriteAsync(buffer.ToArray(), 0, (int)buffer.Length);
                }
            }
        }

        public async Task<bool> AlbumFolderExists(string artist, string album)
        {
            try
            {
                var imageFile =
                    await ApplicationData.Current.LocalFolder.GetFolderAsync(AlbumArtFolderName(artist, album));
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private static string AlbumArtFolderName(string artist, string album)
        {
            return Path.Combine("AlbumArt", artist, album.Replace(":", ""));
        }

        public string AlbumArtFileName(string artist, string album, uint size)
        {
            return Path.Combine(AlbumArtFolderName(artist, album), size + ".jpg");
        }
    }

    public interface IAlbumArtStorage
    {
        string AlbumArtFileName(string artist, string album, uint size);
        Task SaveBitmapAsync(string artist, string album, uint size, string songPath);

        Task<bool> AlbumFolderExists(string artist, string album);
    }
}