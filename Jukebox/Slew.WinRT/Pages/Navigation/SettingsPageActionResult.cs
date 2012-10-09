using Slew.WinRT.Pages.Settings;

namespace Slew.WinRT.Pages.Navigation
{
    public class SettingsPageActionResult<T> : PageActionResult<T>
        where T : SettingsView
    {
        public SettingsPageActionResult(object parameter) : base(parameter)
        {}
    }
}