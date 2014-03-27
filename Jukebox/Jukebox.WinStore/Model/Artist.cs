using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Slab.Data;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Model
{
    [DebuggerDisplay("Artist - {Name}")]
	public class Artist : BindableBase
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
		}

        public string SmallBitmapUri
        {
            get { return Albums.First().SmallBitmapUri; }
        }

        public string LargeBitmapUri
        {
            get { return Albums.First().LargeBitmapUri; }
        }
	}
}