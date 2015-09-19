using Windows.UI.Xaml;
using Orienteer.WinStore.Pages;
using PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PositionTransformRequest : PresentationRequest<PositionTransformRequest, PositionTransformResponse>
    {
        public PositionTransformRequest(FrameworkElement element)
        {
            Element = element;
        }

        public FrameworkElement Element { get; set; }
    }

    public class PositionTransformResponse : IPresentationResponse
    {
        public Location Location { get; set; }
    }
}