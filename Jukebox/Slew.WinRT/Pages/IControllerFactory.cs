namespace Slew.WinRT.Pages
{
    public interface IControllerFactory
    {
        TController Create<TController>() where TController : IController, new();
    }
}