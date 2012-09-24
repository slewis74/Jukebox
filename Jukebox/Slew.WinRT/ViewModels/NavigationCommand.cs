using System;
using Slew.WinRT.Pages;

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
}