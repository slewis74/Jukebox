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

        public IViewResolver ViewResolver { get; set; }
        
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

            SwitchedContent.Content = ViewResolver.Resolver(pageViewModel, ApplicationView.Value);
        }
    }
}
