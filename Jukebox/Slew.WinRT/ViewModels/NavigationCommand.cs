﻿using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.ViewModels
{
    public abstract class NavigationCommand<T> : Command<T>
    {
        protected NavigationCommand(INavigator navigator)
        {
            Navigator = navigator;
        }

        protected INavigator Navigator { get; private set; }
    }

    public abstract class NavigationCommand : Command
    {
        protected NavigationCommand(INavigator navigator)
        {
            Navigator = navigator;
        }

        protected INavigator Navigator { get; private set; }
    }
}