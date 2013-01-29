using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Requests
{
    public class PageNavigationRequest : PresentationRequest<PageNavigationRequestEventArgs>
    {
        public PageNavigationRequest(PageNavigationRequestEventArgs args) : base(args)
        {
        }
    }
}