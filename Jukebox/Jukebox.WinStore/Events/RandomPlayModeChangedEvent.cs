using Slab.PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class RandomPlayModeChangedEvent : PresentationEvent<bool>
    {
        public RandomPlayModeChangedEvent(bool data) : base(data)
        {
        }
    }
}