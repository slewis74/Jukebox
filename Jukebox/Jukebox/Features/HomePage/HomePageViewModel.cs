using Jukebox.Features.HomePage.PlaylistSummary;
using Slab.Data;

namespace Jukebox.Features.HomePage
{
    public class HomePageViewModel : BindableBase
    {
        public HomePageViewModel()
        {
            PlaylistSummary = new PlaylistSummaryViewModel();
        }

        public PlaylistSummaryViewModel PlaylistSummary { get; private set; }
    }
}