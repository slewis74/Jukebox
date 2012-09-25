using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Windows.UI.Xaml;

namespace Jukebox.Requests
{
    public class PositionTransformRequest : PresentationRequest<FrameworkElement>
    {
        public PositionTransformRequest(FrameworkElement args) : base(args)
        {
            MustBeHandled = true;
        }

        public Location Location { get; set; }
    }
}