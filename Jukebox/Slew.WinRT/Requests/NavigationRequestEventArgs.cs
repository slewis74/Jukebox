using System;

namespace Slew.WinRT.Requests
{
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