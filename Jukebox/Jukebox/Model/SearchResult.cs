using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Model
{
    public class SearchResult
    {
        public SearchResultType Type { get; set; }
        public string Description { get; set; }
        public string NavigationUri { get; set; }

        public BitmapImage SmallBitmap { get; set; }
    }

    public enum SearchResultType
    {
        Album = 1,
        Artist = 2,
        Song = 3
    }
}