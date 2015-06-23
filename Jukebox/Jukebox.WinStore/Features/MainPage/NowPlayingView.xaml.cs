using System.Threading.Tasks;
using Jukebox.WinStore.Requests;
using Orienteer.WinStore.Pages;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Features.MainPage
{
    public sealed partial class NowPlayingView :
        IHandlePresentationEventAsync<PlayDropLocationRequest>
    {
        public NowPlayingView()
        {
            InitializeComponent();
        }

        public IPresentationBus PresentationBus { get; set; }

        public async Task HandleAsync(PlayDropLocationRequest request)
        {
            request.IsHandled = true;
            request.Location = await GetPlayDropLocation();
        }

        private async Task<Location> GetPlayDropLocation()
        {
            var request = new PositionTransformRequest(playPauseGrid);
            await PresentationBus.PublishAsync(request);
            return request.Location;
        }
    }
}
