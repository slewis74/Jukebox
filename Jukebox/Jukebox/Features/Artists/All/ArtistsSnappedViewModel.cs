using Jukebox.Model;
using Slew.WinRT.Data;

namespace Jukebox.Features.Artists.All
{
    public class ArtistsSnappedViewModel : BindableBase
    {
        private readonly DistinctAsyncObservableCollection<Artist> _artists;

        public ArtistsSnappedViewModel(
            DistinctAsyncObservableCollection<Artist> artists,
            DisplayArtistCommand displayArtist,
            PlayAllCommand playAll)
        {
            DisplayArtist = displayArtist;
            PlayAll = playAll;
            _artists = artists;
        }

        public DisplayArtistCommand DisplayArtist { get; private set; }
        public PlayAllCommand PlayAll { get; private set; }

        public DistinctAsyncObservableCollection<Artist> Items { get { return _artists; } }
    }
}