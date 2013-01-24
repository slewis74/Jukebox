using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Pages
{
    public class Controller : IController
    {
        public IPresentationBus PresentationBus { get; set; }
        public INavigator Navigator { get; set; }
    }
}