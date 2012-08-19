using Slew.WinRT.Pages;

namespace Jukebox.Common
{
    public class JukeboxControllerFactory : ControllerFactory
    {
        private readonly IHandlePlaylists _handlesPlaylists;

        public JukeboxControllerFactory(IHandlePlaylists handlesPlaylists)
        {
            _handlesPlaylists = handlesPlaylists;
        }

        public override TController Create<TController>()
        {
            var controller = base.Create<TController>();

            var jukeboxController = controller as JukeboxController;
            jukeboxController.HandlesPlaylists = _handlesPlaylists;

            return controller;
        }
    }
}