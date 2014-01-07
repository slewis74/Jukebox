using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Slab.Data;

namespace Jukebox.Model
{
    [DebuggerDisplay("Song - {TrackNumber} {Title}")]
    public class Song : BindableBase
    {
        public Song(SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {
            DiscNumber = 1;
        }

        public uint DiscNumber { get; set; }
        public uint TrackNumber { get; set; }
        
        public string Title { get; set; }
        public string Path { get; set; }

        public TimeSpan Duration { get; set; }

        private Album _album;
        public Album Album
        {
            get { return _album; }
            set { _album = value; _album.Songs.Add(this);}
        }

        private StorageFile _storageFile;

        public async Task<StorageFile> GetStorageFileAsync()
        {
            return _storageFile ?? (_storageFile = await StorageFile.GetFileFromPathAsync(Path));
        }

        public void SetStorageFile(StorageFile storageFile)
        {
            _storageFile = storageFile;
        }
    }
}