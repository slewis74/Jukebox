using System;
using Autofac;
using Slew.WinRT.Pages;

namespace Jukebox.Common
{
    public class JukeboxControllerFactory : ControllerFactory
    {
        private readonly IComponentContext _componentContext;

        public JukeboxControllerFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public override TController Create<TController>()
        {
            var controller = (TController)_componentContext.Resolve(typeof(TController));
            return controller;
        }
    }
}