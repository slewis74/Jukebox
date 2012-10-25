using Slew.WinRT.PresentationBus;

namespace Jukebox.Events
{
    public class RandomPlayModeChangedEvent : PresentationEvent<bool>
    {
        public RandomPlayModeChangedEvent(bool data) : base(data)
        {
        }
    }
}