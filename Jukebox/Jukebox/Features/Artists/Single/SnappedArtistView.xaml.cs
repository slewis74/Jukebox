using Jukebox.Model;
using Windows.UI.Xaml.Controls;

namespace Jukebox.Features.Artists.Single
{
    public sealed partial class SnappedArtistView
    {
        public SnappedArtistView()
        {
            InitializeComponent();
        }

        private void ItemClicked(object sender, ItemClickEventArgs e)
        {
            var album = e.ClickedItem as Album;
            if (album == null) return;

            var artistViewModel = (ArtistViewModel)DataContext;

            artistViewModel.DisplayAlbum.Execute(album);
        }
    }
}
