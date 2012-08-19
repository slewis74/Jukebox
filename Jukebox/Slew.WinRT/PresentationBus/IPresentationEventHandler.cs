namespace Slew.WinRT.PresentationBus
{
    public interface IPresentationEventHandler
    {
        void Handle(IPresentationEvent presentationEvent);
    }

    public interface IPresentationEventHandler<in T>
    {
        void Handle(IPresentationEvent<T> presentationEvent);
    }
}