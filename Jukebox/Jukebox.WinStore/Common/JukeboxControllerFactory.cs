using System;
using Autofac;
using Orienteer.Pages;

namespace Jukebox.WinStore.Common
{
    public class JukeboxControllerFactory : ControllerFactory
    {
        private readonly IComponentContext _componentContext;

        public JukeboxControllerFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public override object Create(Type controllerType)
        {
            return _componentContext.Resolve(controllerType);
        }
    }
}