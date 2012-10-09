using Slew.WinRT.Container;
using Slew.WinRT.Pages.Settings;

namespace Slew.WinRT.Pages.Navigation
{
    public class SettingsPageActionResult<TView, TViewModel> : PageActionResult<TView>
        where TView : SettingsView
        where TViewModel : new()
    {
        public SettingsPageActionResult() : base(PropertyInjector.Inject(() => new TViewModel()))
        {}
    }
}