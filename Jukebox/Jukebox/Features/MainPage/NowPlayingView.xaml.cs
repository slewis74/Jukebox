using Jukebox.Requests;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Features.MainPage
{
    public sealed partial class NowPlayingView :
        IPublish,
        IHandlePresentationRequest<PlayDropLocationRequest>
    {
        public NowPlayingView()
        {
            InitializeComponent();
        }

        public IPresentationBus PresentationBus { get; set; }

        public void Handle(PlayDropLocationRequest request)
        {
            request.IsHandled = true;
            request.Location = GetPlayDropLocation();
        }

        private Location GetPlayDropLocation()
        {
            var request = new PositionTransformRequest(playPauseGrid);
            PresentationBus.Publish(request);
            return request.Location;
        }
    }
}
