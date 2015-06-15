using Jukebox.WinStore.Features.HomePage.PlaylistSummary;
using Orienteer.Data;

namespace Jukebox.WinStore.Features.HomePage
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