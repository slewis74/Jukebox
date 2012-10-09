namespace Slew.WinRT.Pages
{
    public class ControllerFactory : IControllerFactory
    {
        public virtual TController Create<TController>() where TController : IController, new()
        {
            var controller = new TController();
            return controller;
        }
    }
}