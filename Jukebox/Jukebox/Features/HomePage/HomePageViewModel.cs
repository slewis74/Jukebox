using Jukebox.Features.HomePage.PlaylistSummary;
using Slew.WinRT.Data;

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