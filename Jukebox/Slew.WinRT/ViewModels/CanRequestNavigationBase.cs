using Slew.WinRT.Data;
using Slew.WinRT.Pages;

namespace Slew.WinRT.ViewModels
{
    public abstract class CanRequestNavigationBase : BindableBase, ICanRequestNavigation
    {
        public INavigator Navigator { get; set; }
    }
}