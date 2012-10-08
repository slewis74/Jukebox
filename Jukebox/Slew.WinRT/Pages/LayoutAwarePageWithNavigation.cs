using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Slew.WinRT.Pages
{
	public class LayoutAwarePageWithNavigation : LayoutAwarePage
	{
		public event EventHandler<FrameworkElement> ProvideAppBarContent;

		protected override void GoHome(object sender, RoutedEventArgs e)
		{
			if (Frame == null) return;
			while (Frame.CanGoBack) Frame.GoBack();
		}

        protected override void GoBack(object sender, RoutedEventArgs e)
		{
			if (Frame != null && Frame.CanGoBack) Frame.GoBack();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter != null)
			{
				DataContext = e.Parameter;
			}

		    FrameworkElement content = null;
		    var haveBottomAppBar = this as IHaveBottomAppBar;
		    if (haveBottomAppBar != null)
		    {
		        var contentType = haveBottomAppBar.BottomAppBarContentType;
		        content = (FrameworkElement)Activator.CreateInstance(contentType);
		        content.DataContext = DataContext;
		    }
            OnProvideAppBarContent(content);
        }

		private void OnProvideAppBarContent(FrameworkElement frameworkElement)
		{
			if (ProvideAppBarContent == null) return;
			ProvideAppBarContent(this, frameworkElement);
		}
	}
}