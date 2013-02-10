using System;

namespace Slew.WinRT.Pages
{
    public class ControllerFactory : IControllerFactory
    {
        public virtual object Create(Type controllerType)
        {
            var controller = Activator.CreateInstance(controllerType);
            return controller;
        }

        public TController Create<TController>() where TController : IController
        {
            return (TController)Create(typeof(TController));
        }
    }
}