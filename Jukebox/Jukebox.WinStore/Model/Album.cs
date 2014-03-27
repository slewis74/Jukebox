using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Slab.Data;

namespace Jukebox.WinStore.Model
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

        private string _smallBitmapUri;
        public string SmallBitmapUri
        {
            get { return _smallBitmapUri; }
            set { _smallBitmapUri = value; NotifyChanged(() => SmallBitmapUri); }
        }

        private string _largeBitmapUri;
        public string LargeBitmapUri
        {
            get { return _largeBitmapUri; }
            set { _largeBitmapUri = value; NotifyChanged(() => LargeBitmapUri); }
        }
	}
}