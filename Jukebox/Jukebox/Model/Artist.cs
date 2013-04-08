using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Jukebox.Events;
using Slab.Data;
using Slab.PresentationBus;
using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Model
{
    [DebuggerDisplay("Artist - {Name}")]
	public class Artist : BindableBase
    {
        private readonly IPresentationBus _presentationBus;

        public Artist(IPresentationBus presentationBus, SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {
            _presentationBus = presentationBus;
            Albums = new ObservableCollection<Album>();
        }

        public string Name { get; set; }

        public ObservableCollection<Album> Albums { get; private set; }

        public IPresentationBus PresentationBus { get; set; }

		public void AddAlbum(Album album)
		{
			if (Albums.Contains(album))
				return;
			Albums.Add(album);
            _presentationBus.Publish(new AlbumAddedEvent(this, album));
		}

        public BitmapImage SmallBitmap
        {
            get { return Albums.First().SmallBitmap; }
        }

        public BitmapImage LargeBitmap
        {
            get { return Albums.First().LargeBitmap; }
        }
	}
}