using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;

namespace Slew.WinRT.Requests
{
    public class NavigationRequest : PresentationRequest<NavigationRequestEventArgs>
    {
        public NavigationRequest(NavigationRequestEventArgs args) : base(args)
        {
        }
    }
}