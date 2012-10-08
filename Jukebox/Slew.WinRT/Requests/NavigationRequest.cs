using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Requests
{
    public class NavigationRequest : PresentationRequest<NavigationRequestEventArgs>
    {
        public NavigationRequest(NavigationRequestEventArgs args) : base(args)
        {
        }
    }
}