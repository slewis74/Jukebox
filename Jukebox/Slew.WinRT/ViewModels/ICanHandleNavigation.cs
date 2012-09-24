using System;
using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.ViewModels
{
    public interface ICanHandleNavigation : IPublish
    {
        void Navigate(Type viewType);
        void Navigate(Type viewType, object parameter);
    }
}