using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Slew.WinRT.Data;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Model
{
    [DebuggerDisplay("Album - {Title}")]
    public class Album : BindableBase
	{
        public Album(SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {
            Songs = new ObservableCollection<Song>();
        }

        public string Title { get; set; }

        private Artist _artist;
		public Artist Artist
		{
			get { return _artist; }
			set
			{
				_artist = value;
				_artist.AddAlbum(this);
			}
		}

		public ObservableCollection<Song> Songs { get; private set; }

        private bool _loadingSmallBitmap;
        private bool _loadingLargeBitmap;

		public void AddSong(Song song)
		{
			if (Songs.Contains(song))
				return;
			Songs.Add(song);

            if (_smallBitmap == null && _loadingSmallBitmap == false)
            {
                _loadingSmallBitmap = true;
                SynchronizationContext.Post(x =>
                                                {
                                                    SmallBitmap = new BitmapImage();
                                                    GetBitmapAsync(_smallBitmap, 200);
                                                    _loadingSmallBitmap = false;
                                                }, null);
            }
            if (_largeBitmap == null && _loadingLargeBitmap == false)
            {
                _loadingLargeBitmap = true;
                SynchronizationContext.Post(x =>
                                                {
                                                    LargeBitmap = new BitmapImage();
                                                    GetBitmapAsync(_largeBitmap, 300);
                                                    _loadingLargeBitmap = false;
                                                }, null);
            }
		}

        private BitmapImage _smallBitmap;
        public BitmapImage SmallBitmap
        {
            get { return _smallBitmap; }
            set { _smallBitmap = value; NotifyChanged(() => SmallBitmap); }
        }

        private BitmapImage _largeBitmap;
        public BitmapImage LargeBitmap
        {
            get { return _largeBitmap; }
            set { _largeBitmap = value; NotifyChanged(() => LargeBitmap); }
        }

        private async void GetBitmapAsync(BitmapImage image, uint requestedSize)
        {
            using (var randomAccessStream = await GetThumbnailStreamAsync(this, requestedSize))
            {
                if (randomAccessStream == null)
                    return;

                image.SetSource(randomAccessStream);
            }
        }

        private async Task<IRandomAccessStream> GetThumbnailStreamAsync(Album album, uint reqestedSize)
        {
            var storageFile = await album.Songs.First().GetStorageFileAsync();

            using (var thumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.MusicView, reqestedSize) ??
                                   await storageFile.GetThumbnailAsync(ThumbnailMode.VideosView, reqestedSize))
            {
                if (thumbnail == null)
                    return null;

                var reader = new DataReader(thumbnail);
                var fileLength = (uint) thumbnail.Size;
                await reader.LoadAsync(fileLength);

                var buffer = reader.ReadBuffer(fileLength);

                var memStream = new InMemoryRandomAccessStream();

                await memStream.WriteAsync(buffer);
                await memStream.FlushAsync();
                memStream.Seek(0);

                return memStream;
            }
        }

	}
}