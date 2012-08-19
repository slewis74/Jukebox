using System;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;

namespace Slew.WinRT.ViewModels
{
	public abstract class CanRequestNavigationBase : BindableBase, ICanRequestNavigation
	{
        public INavigator Navigator { get; set; }
	}

    public abstract class CanHandleNavigationBase : BindableBase, ICanHandleNavigation
    {
        public event EventHandler<NavigationRequestEventArgs> NavigateRequest;

        public void Navigate(Type viewType)
        {
            if (NavigateRequest == null)
                return;
            NavigateRequest(this, new NavigationRequestEventArgs(viewType));
        }

        public void Navigate(Type viewType, object parameter)
        {
            if (NavigateRequest == null)
                return;
            NavigateRequest(this, new NavigationRequestEventArgs(viewType, parameter));
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