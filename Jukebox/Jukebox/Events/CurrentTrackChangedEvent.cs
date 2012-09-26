using Jukebox.Model;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Events
{
    public class CurrentTrackChangedEvent : PresentationEvent<Song>
    {
        public CurrentTrackChangedEvent(Song data) : base(data)
        {
        }
    }
}