namespace Slew.WinRT.PresentationBus
{
    public interface IPresentationRequest : IPresentationEvent
    {
        bool IsHandled { get; set; }
    }
}