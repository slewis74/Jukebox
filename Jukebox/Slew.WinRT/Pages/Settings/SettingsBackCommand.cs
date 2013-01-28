using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Slew.WinRT.Pages.Settings
{
    public class SettingsBackCommand : Command
    {
        private readonly INavigator _navigator;

        public SettingsBackCommand(INavigator navigator)
        {
            _navigator = navigator;
        }

        public override void Execute(object parameter)
        {
            _navigator.SettingsNavigateBack();
        }
    }
}