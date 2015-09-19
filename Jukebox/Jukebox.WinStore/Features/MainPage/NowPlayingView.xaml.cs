using System.Threading.Tasks;
using Jukebox.WinStore.Requests;
using Orienteer.WinStore.Pages;
using PresentationBus;

namespace Jukebox.WinStore.Features.MainPage
{
    public sealed partial class NowPlayingView :
        IHandlePresentationRequestAsync<PlayDropLocationRequest, PlayDropLocationResponse>
    {
        public NowPlayingView()
        {
            InitializeComponent();
        }

        public IPresentationBus PresentationBus { get; set; }

        public async Task<PlayDropLocationResponse> HandleAsync(PlayDropLocationRequest request)
        {
            var location = await GetPlayDropLocation();
            return new PlayDropLocationResponse {Location = location};
        }

        private async Task<Location> GetPlayDropLocation()
        {
            var request = new PositionTransformRequest(playPauseGrid);
            var response = await PresentationBus.Request(request);
            return response.Location;
        }
    }
}
