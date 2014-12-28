using Slew.PresentationBus;

namespace Jukebox.WinStore.Requests
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