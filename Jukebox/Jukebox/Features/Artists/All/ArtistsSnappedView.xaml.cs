using Jukebox.Model;
using Windows.UI.Xaml;

namespace Jukebox.Features.Artists.All
{
    public sealed partial class ArtistsSnappedView
    {
        public ArtistsSnappedView()
        {
            InitializeComponent();
        }

        private void MoreClicked(object sender, RoutedEventArgs e)
        {
            var artist = ((FrameworkElement)sender).DataContext as Artist;
            if (artist == null) return;

            var viewModel = (ArtistsSnappedViewModel)DataContext;
            viewModel.DisplayArtist.Execute(artist);
        }
    }
}
