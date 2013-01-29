using Slew.WinRT.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Slew.WinRT.Pages
{
    public interface IViewResolver
    {
        FrameworkElement Resolve(ViewModelWithOrientation viewModel, ApplicationViewState applicationViewState);
    }
}