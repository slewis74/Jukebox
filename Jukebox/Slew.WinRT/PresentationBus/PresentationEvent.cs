namespace Slew.WinRT.PresentationBus
{
    public class PresentationEvent : IPresentationEvent
    {
        public bool IsHandled { get; set; }
        public bool MustBeHandled { get; protected set; }
    }

    public class PresentationEvent<T> : PresentationEvent, IPresentationEvent<T>
    {
        public T Data { get; set; }
    }
}