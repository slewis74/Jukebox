using Windows.UI.Xaml;
using Orienteer.WinStore.Pages;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class PositionTransformRequest : PresentationRequest
    {
        public PositionTransformRequest(FrameworkElement element)
        {
            Element = element;
            MustBeHandled = true;
        }

        public FrameworkElement Element { get; set; }
        public Location Location { get; set; }
    }
}