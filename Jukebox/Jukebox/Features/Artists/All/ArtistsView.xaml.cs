using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Slab.WinStore.Pages;

namespace Jukebox.Features.Artists.All
{
	public sealed partial class ArtistsView : IHaveBottomAppBar
	{
		public ArtistsView()
		{
			InitializeComponent();
            Loaded += ArtistsViewLoaded;
        }

        void ArtistsViewLoaded(object sender, RoutedEventArgs e)
        {
            var listViewBase = SemanticZoomControl.ZoomedOutView as ListViewBase;
            if (listViewBase != null)
                listViewBase.ItemsSource = GroupedItemsViewSource.View.CollectionGroups;
        }

	    private void MoreClicked(object sender, RoutedEventArgs e)
		{
			var artist = ((FrameworkElement)sender).DataContext as GroupedArtistViewModel;
			if (artist == null) return;

			var viewModel = (ArtistsViewModel)DataContext;
			viewModel.DisplayArtist.Execute(artist.Artist);
		}

	    public Type BottomAppBarContentType { get { return typeof (ArtistsBottomAppBarView); } }
	}
}
