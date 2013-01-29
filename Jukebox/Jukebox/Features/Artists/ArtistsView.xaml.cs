using System;
using Slew.WinRT.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Jukebox.Features.Artists
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
