using Slab.PresentationBus;

namespace Jukebox.Events
{
    public class RandomPlayModeChangedEvent : PresentationEvent<bool>
    {
        public RandomPlayModeChangedEvent(bool data) : base(data)
        {
        }
    }
}