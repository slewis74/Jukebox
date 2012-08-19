﻿using System;
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

		public void AddSong(Song song)
		{
			if (Songs.Contains(song))
				return;
			Songs.Add(song);

            if (_smallBitmap == null)
            {
                SynchronizationContext.Post(x =>
                                                {
                                                    SmallBitmap = new BitmapImage();
                                                    GetBitmap(_smallBitmap, 64);
                                                }, null);
            }
            if (_largeBitmap == null)
            {
                SynchronizationContext.Post(x =>
                                                {
                                                    LargeBitmap = new BitmapImage();
                                                    GetBitmap(_largeBitmap, 256);
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

        private async void GetBitmap(BitmapImage image, uint requestedSize)
        {
            var randomAccessStream = await GetThumbnailStream(this, requestedSize);
            if (randomAccessStream == null)
                return;

            image.SetSource(randomAccessStream);
        }

        private async Task<IRandomAccessStream> GetThumbnailStream(Album album, uint reqestedSize)
        {
            var storageFile = await album.Songs.First().GetStorageFileAsync();

            var thumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.MusicView, reqestedSize) ??
                            await storageFile.GetThumbnailAsync(ThumbnailMode.VideosView, reqestedSize);
            if (thumbnail == null)
                return null;

            var reader = new DataReader(thumbnail);
            var fileLength = (uint)thumbnail.Size;
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