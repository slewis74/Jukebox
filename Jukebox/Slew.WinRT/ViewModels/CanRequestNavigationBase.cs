using System;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.Requests;

namespace Slew.WinRT.ViewModels
{
	public abstract class CanRequestNavigationBase : BindableBase, ICanRequestNavigation
	{
        public INavigator Navigator { get; set; }
	}

    public abstract class CanHandleNavigationBase : BindableBase, ICanHandleNavigation
    {
        public event EventHandler<NavigationRequestEventArgs> NavigateRequest;

        public IPresentationBus PresentationBus { get; set; }

        public void Navigate(Type viewType)
        {
            var navigationRequestEventArgs = new NavigationRequestEventArgs(viewType);
            if (PresentationBus != null)
            {
                PresentationBus.Publish(new NavigationRequest(navigationRequestEventArgs));
            }
            
            if (NavigateRequest == null)
                return;
            NavigateRequest(this, navigationRequestEventArgs);
        }

        public void Navigate(Type viewType, object parameter)
        {
            var navigationRequestEventArgs = new NavigationRequestEventArgs(viewType, parameter);
            if (PresentationBus != null)
            {
                PresentationBus.Publish(new NavigationRequest(navigationRequestEventArgs));
            }
            
            if (NavigateRequest == null)
                return;
            NavigateRequest(this, navigationRequestEventArgs);
        }
    }

    public class NavigationRequestEventArgs : EventArgs
    {
        public NavigationRequestEventArgs(Type viewType)
        {
            ViewType = viewType;
        }

        public NavigationRequestEventArgs(Type viewType, object parameter)
        {
            ViewType = viewType;
            Parameter = parameter;
        }

        public Type ViewType { get; private set; }
        public object Parameter { get; private set; }
    }
}