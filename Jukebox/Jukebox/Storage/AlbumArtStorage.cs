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

                // http://social.msdn.microsoft.com/Forums/windowsapps/en-US/1dda3a15-d299-40e0-b668-ec690a683f6e/how-to-resize-an-image-as-storagefile?forum=winappswithcsharp
                var decoder = await BitmapDecoder.CreateAsync(memStream);
                var transform = new BitmapTransform { ScaledHeight = size, ScaledWidth = size };
                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                using (var destinationStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destinationStream);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, size, size, 96, 96, pixelData.DetachPixelData());
                    await encoder.FlushAsync();
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
            return Path.Combine(AlbumArtFolderName(artist, album), "AlbumArt" + size + "x" + size + ".jpg");
        }
    }

    public interface IAlbumArtStorage
    {
        string AlbumArtFileName(string artist, string album, uint size);
        Task SaveBitmapAsync(string artist, string album, uint size, string songPath);

        Task<bool> AlbumFolderExists(string artist, string album);
    }
}