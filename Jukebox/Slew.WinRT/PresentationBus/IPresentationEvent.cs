namespace Slew.WinRT.PresentationBus
{
    public interface IPresentationEvent
    {
        object Data { get; }
    }

    public interface IPresentationEvent<out T>
    {
        T Data { get; }
    }
}