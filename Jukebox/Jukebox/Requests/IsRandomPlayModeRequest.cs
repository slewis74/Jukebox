using Slew.WinRT.PresentationBus;

namespace Jukebox.Requests
{
    public class IsRandomPlayModeRequest : PresentationRequest
    {
        public IsRandomPlayModeRequest()
        {
            MustBeHandled = true;
        }

        public bool IsRandomPlayMode { get; set; }
    }
}