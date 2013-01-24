using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Slew.WinRT.Pages.Settings
{
    public class SettingsPopupViewModel : BindableBase
    {
        public SettingsPopupViewModel()
        {
            Back = new SettingsBackCommand();
        }

        public SettingsBackCommand Back { get; private set; }
    }

    public class SettingsBackCommand : 
        Command
    {
        public INavigator Navigator { get; set; }

        public override void Execute(object parameter)
        {
            Navigator.SettingsNavigateBack();
        }
    }
}