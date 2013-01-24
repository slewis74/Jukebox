using System;
using Slew.WinRT.ViewModels;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Slew.WinRT.Pages
{
    public sealed partial class ContentSwitchingPage
    {
        public ContentSwitchingPage()
        {
            InitializeComponent();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            Loaded += StartLayoutUpdates;

            Unloaded += StopLayoutUpdates;
        }

        private void StartLayoutUpdates(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += WindowSizeChanged;
            WindowSizeChanged(this, null);
        }

        private void StopLayoutUpdates(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= WindowSizeChanged;
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            var pageViewModel = DataContext as ViewModelWithOrientation;

            if (pageViewModel == null) return;

            FrameworkElement view;
            object childViewModel;
            switch (ApplicationView.Value)
            {
                case ApplicationViewState.Snapped:
                    view = (FrameworkElement)Activator.CreateInstance(pageViewModel.SnappedViewType);
                    childViewModel = pageViewModel.SnappedViewModel;
                    break;
                case ApplicationViewState.Filled:
                    view = (FrameworkElement)Activator.CreateInstance(pageViewModel.FilledViewType);
                    childViewModel = pageViewModel.FilledViewModel;
                    break;
                case ApplicationViewState.FullScreenPortrait:
                    view = (FrameworkElement)Activator.CreateInstance(pageViewModel.PortraitViewType);
                    childViewModel = pageViewModel.PortraitViewModel;
                    break;
                //case ApplicationViewState.FullScreenLandscape:
                default:
                    view = (FrameworkElement)Activator.CreateInstance(pageViewModel.LandscapeViewType);
                    childViewModel = pageViewModel.LandscapeViewModel;
                    break;
            }

            view.DataContext = childViewModel;
            SwitchedContent.Content = view;
        }
    }
}
