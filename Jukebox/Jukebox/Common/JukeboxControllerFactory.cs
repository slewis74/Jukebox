using Slew.WinRT.Pages;

namespace Jukebox.Common
{
    public class JukeboxControllerFactory : ControllerFactory
    {
        public override TController Create<TController>()
        {
            var controller = base.Create<TController>();
            return controller;
        }
    }
}