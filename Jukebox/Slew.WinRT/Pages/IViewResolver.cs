using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Slew.WinRT.Pages
{
    public interface IViewResolver
    {
        FrameworkElement Resolve(object viewModel, ApplicationViewState applicationViewState);
    }
}