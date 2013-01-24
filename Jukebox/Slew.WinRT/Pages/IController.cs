using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Pages
{
    public interface IController
    {
        IPresentationBus PresentationBus { get; set; }
        INavigator Navigator { get; set; }
    }
}