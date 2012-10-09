using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.ViewModels
{
    public abstract class CanRequestNavigationBase : BindableBase, ICanRequestNavigation
    {
        public INavigator Navigator { get; set; }
    }
}