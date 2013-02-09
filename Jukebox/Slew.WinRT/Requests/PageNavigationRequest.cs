using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Requests
{
    public class PageNavigationRequest : PresentationRequest<PageNavigationRequestEventArgs>
    {
        public PageNavigationRequest(string uri, PageNavigationRequestEventArgs args) : base(args)
        {
            Uri = uri;
        }

        public string Uri { get; set; }
    }
}