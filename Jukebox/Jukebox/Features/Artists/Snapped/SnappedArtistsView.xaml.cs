﻿using Jukebox.Model;
using Windows.UI.Xaml;

namespace Jukebox.Features.Artists.Snapped
{
    public sealed partial class SnappedArtistsView
    {
        public SnappedArtistsView()
        {
            InitializeComponent();
        }

        private void MoreClicked(object sender, RoutedEventArgs e)
        {
            var artist = ((FrameworkElement)sender).DataContext as Artist;
            if (artist == null) return;

            var viewModel = (ArtistsViewModel)DataContext;
            viewModel.DisplayArtist.Execute(artist);
        }
    }
}
