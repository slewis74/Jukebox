using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.Pages
{
    public interface IController
    {
        INavigator Navigator { get; set; }
    }
}