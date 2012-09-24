namespace Slew.WinRT.PresentationBus
{
    public interface IHandlePresentationEvent<in T> : IHandlePresentationEvents
        where T : IPresentationEvent
    {
        void Handle(T presentationEvent);
    }
}