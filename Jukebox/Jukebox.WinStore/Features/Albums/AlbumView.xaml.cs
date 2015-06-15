using System;
using Orienteer.WinStore.Pages;

namespace Jukebox.WinStore.Features.Albums
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
