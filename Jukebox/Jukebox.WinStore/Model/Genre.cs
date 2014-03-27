using System.Collections.ObjectModel;
using System.Diagnostics;
using Slab.Data;

namespace Jukebox.WinStore.Model
{
    [DebuggerDisplay("Genre - {Name}")]
    public class Genre : BindableBase
	{
		public Genre()
		{
			Albums = new ObservableCollection<Album>();
		}

		public string Name { get; set; }

		public ObservableCollection<Album> Albums { get; set; } 
	}
}