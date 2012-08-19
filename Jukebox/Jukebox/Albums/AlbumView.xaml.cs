using System;
using Slew.WinRT.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Jukebox.Albums
{
	public sealed partial class AlbumView : IHaveBottomAppBar
	{
        public AlbumView()
		{
			InitializeComponent();

            Loaded += AlbumViewLoaded;
		}

		public Type BottomAppBarContentType
		{
			get { return typeof (AlbumViewBottomAppBar); }
		}

        void AlbumViewLoaded(object sender, RoutedEventArgs e)
        {
            var parent = GetTopLevelParent() as Page;
            var z = parent as IAcceptPlaylistDragging;

            ((AlbumViewModel) DataContext).SetLocations(z.GetPlayDropLocation(), z.GetPlaylistDropLocation());
        }

        private FrameworkElement GetTopLevelParent()
        {
            var parent = Parent as FrameworkElement;
            while (parent != null && parent is IAcceptPlaylistDragging == false)
            {
                parent = parent.Parent as FrameworkElement;
            }
            return parent;
        }
	}
}
