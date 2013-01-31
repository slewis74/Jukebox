using System;

namespace Slew.WinRT.Pages
{
    public class ControllerFactory : IControllerFactory
    {
        public virtual TController Create<TController>() where TController : IController
        {
            var controller = Activator.CreateInstance<TController>();
            return controller;
        }
    }
}