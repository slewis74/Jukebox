using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.Pages
{
    public class Controller : IController
    {
        public INavigator Navigator { get; set; }
    }
}