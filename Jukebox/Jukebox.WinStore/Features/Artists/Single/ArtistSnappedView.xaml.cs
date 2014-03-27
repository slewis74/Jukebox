using Windows.UI.Xaml.Controls;
using Jukebox.WinStore.Model;

namespace Jukebox.WinStore.Features.Artists.Single
{
    public sealed partial class ArtistSnappedView
    {
        public ArtistSnappedView()
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
