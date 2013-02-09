using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Slew.WinRT.Pages
{
    public interface IViewLocator
    {
        FrameworkElement Resolve(object viewModel, ApplicationViewState applicationViewState);
    }
}