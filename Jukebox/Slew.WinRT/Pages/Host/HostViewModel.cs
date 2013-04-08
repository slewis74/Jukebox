using Slew.WinRT.Data;
using Slew.WinRT.Data.Navigation;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Pages.Host
{
    public class HostViewModel : BindableBase
    {
        public HostViewModel(string defaultRoute)
        {
            DefaultRoute = defaultRoute;
        }

        public IPresentationBus PresentationBus { get; set; }
        public INavigator Navigator { get; set; }
        public IViewLocator ViewLocator { get; set; }
        public INavigationStackStorage NavigationStackStorage { get; set; }
        public IControllerInvoker ControllerInvoker { get; set; }
        public string DefaultRoute { get; set; } 
    }
}