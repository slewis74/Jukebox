using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Jukebox.Events;
using Slew.WinRT.Data;
using Slew.WinRT.PresentationBus;
using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Model
{
    [DebuggerDisplay("Artist - {Name}")]
	public class Artist : BindableBase, IPublish
    {
        public Artist(SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {
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
            PresentationBus.Publish(new AlbumAddedEvent(this, album));
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