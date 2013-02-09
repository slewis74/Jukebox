using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Requests
{
    public class ViewModelNavigationRequest : PresentationRequest<ViewModelNavigationRequestEventArgs>
    {
        public ViewModelNavigationRequest(string uri, ViewModelNavigationRequestEventArgs args) : base(args)
        {
            Uri = uri;
        }

        public string Uri { get; set; }
    }
}