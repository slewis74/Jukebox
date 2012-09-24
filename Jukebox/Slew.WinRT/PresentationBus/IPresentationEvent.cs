namespace Slew.WinRT.PresentationBus
{
    public interface IPresentationEvent{}

    public interface IPresentationEvent<out T> : IPresentationEvent
    {
        T Data { get; }
    }
}