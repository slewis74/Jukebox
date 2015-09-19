using PresentationBus;

namespace Jukebox.WinStore.Requests
{
    public class IsRandomPlayModeRequest : PresentationRequest<IsRandomPlayModeRequest, IsRandomPlaymodeResponse>
    {
        public IsRandomPlayModeRequest()
        {
        }
    }

    public class IsRandomPlaymodeResponse : IPresentationResponse
    { 
        public bool IsRandomPlayMode { get; set; }
    }
}