using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Slew.WinRT.Pages
{
    public sealed partial class ContentSwitchingPage
    {
        private readonly Dictionary<ApplicationViewState, FrameworkElement> _viewCache;

        public ContentSwitchingPage()
        {
            InitializeComponent();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            _viewCache = new Dictionary<ApplicationViewState, FrameworkElement>();

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
            var pageViewModel = DataContext;
            if (pageViewModel == null) return;

            FrameworkElement frameworkElement;
            if (_viewCache.ContainsKey(ApplicationView.Value))
                frameworkElement = _viewCache[ApplicationView.Value];
            else
            {
                frameworkElement = ViewResolver.Resolve(pageViewModel, ApplicationView.Value);
                _viewCache.Add(ApplicationView.Value, frameworkElement);
            }

            Content = frameworkElement;
        }
    }
}
