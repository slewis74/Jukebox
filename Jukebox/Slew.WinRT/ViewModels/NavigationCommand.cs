using System;
using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.ViewModels
{
    public abstract class NavigationCommand<T> : Command<T>
    {
        protected NavigationCommand(Lazy<INavigator> navigator)
        {
            Navigator = navigator;
        }

        protected Lazy<INavigator> Navigator { get; private set; }
    }

    public abstract class NavigationCommand : Command
    {
        protected NavigationCommand(Lazy<INavigator> navigator)
        {
            Navigator = navigator;
        }

        protected Lazy<INavigator> Navigator { get; private set; }
    }
}