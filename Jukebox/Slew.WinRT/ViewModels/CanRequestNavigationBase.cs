using System.Threading;
using System.Windows.Input;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.ViewModels
{
    public abstract class CanRequestNavigationBase : BindableBase
    {
        protected CanRequestNavigationBase(INavigator navigator)
        {
            Navigator = navigator;
        }

        protected CanRequestNavigationBase(INavigator navigator, SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {
            Navigator = navigator;
        }

        public INavigator Navigator { get; private set; }
        public ICommand NavigateBackCommand { get; private set; }
    }
}