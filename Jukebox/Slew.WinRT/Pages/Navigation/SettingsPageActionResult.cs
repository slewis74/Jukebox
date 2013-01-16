using Slew.WinRT.Pages.Settings;

namespace Slew.WinRT.Pages.Navigation
{
    public class SettingsPageActionResult<TView, TViewModel> : PageActionResult<TView>, ISettingsPageActionResult where TView : SettingsView
        where TViewModel : new()
    {
        public SettingsPageActionResult() : base(new TViewModel())
        {}
    }
}