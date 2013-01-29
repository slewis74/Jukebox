using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Requests
{
    public class ViewModelNavigationRequest : PresentationRequest<ViewModelNavigationRequestEventArgs>
    {
        public ViewModelNavigationRequest(ViewModelNavigationRequestEventArgs args) : base(args)
        {
        }
    }
}