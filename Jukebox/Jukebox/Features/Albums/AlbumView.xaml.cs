using System;
using Slab.WinStore.Pages;

namespace Jukebox.Features.Albums
{
	public sealed partial class AlbumView : IHaveBottomAppBar
	{
        public AlbumView()
		{
			InitializeComponent();
		}

	    public Type BottomAppBarContentType { get { return typeof (AlbumBottomAppBarView); } }
	}
}
