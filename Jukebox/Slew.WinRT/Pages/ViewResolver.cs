using System;
using Slew.WinRT.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Slew.WinRT.Pages
{
    public class ViewResolver : IViewResolver
    {
        public FrameworkElement Resolve(ViewModelWithOrientation viewModel, ApplicationViewState applicationViewState)
        {
            FrameworkElement view;
            object childViewModel;
            switch (applicationViewState)
            {
                case ApplicationViewState.Snapped:
                    view = (FrameworkElement)Activator.CreateInstance(viewModel.SnappedViewType);
                    childViewModel = viewModel.SnappedViewModel;
                    break;
                case ApplicationViewState.Filled:
                    view = (FrameworkElement)Activator.CreateInstance(viewModel.FilledViewType);
                    childViewModel = viewModel.FilledViewModel;
                    break;
                case ApplicationViewState.FullScreenPortrait:
                    view = (FrameworkElement)Activator.CreateInstance(viewModel.PortraitViewType);
                    childViewModel = viewModel.PortraitViewModel;
                    break;
                    //case ApplicationViewState.FullScreenLandscape:
                default:
                    view = (FrameworkElement)Activator.CreateInstance(viewModel.LandscapeViewType);
                    childViewModel = viewModel.LandscapeViewModel;
                    break;
            }

            view.DataContext = childViewModel;

            return view;
        }
    }
}