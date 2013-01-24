using System;

namespace Slew.WinRT.ViewModels
{
    public interface ICanHandleNavigation
    {
        void Navigate(Type viewType);
        void Navigate(Type viewType, object parameter);
    }
}