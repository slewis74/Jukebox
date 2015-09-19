using PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class RandomPlayModeChangedEvent : PresentationEvent
    {
        public RandomPlayModeChangedEvent(bool isRandomPlayMode)
        {
            IsRandomPlayMode = isRandomPlayMode;
        }

        public bool IsRandomPlayMode { get; set; }
    }
}