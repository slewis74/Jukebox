using Slew.WinRT.Pages;
using Slew.WinRT.ViewModels;

namespace Slew.WinRT.Container
{
    public class CanRequestNavigationInjectorRule : IPropertyInjectorRule
    {
        private readonly INavigator _navigator;

        public CanRequestNavigationInjectorRule(INavigator navigator)
        {
            _navigator = navigator;
        }

        public void Process<T>(T obj)
        {
            var canRequestNavigation = obj as ICanRequestNavigation;
            if (canRequestNavigation != null)
            {
                canRequestNavigation.Navigator = _navigator;
            }
        }
    }
}