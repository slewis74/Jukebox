using Windows.UI.Xaml;

namespace Slew.WinRT.Pages
{
	public class LayoutAwarePageWithNavigation : LayoutAwarePage
	{
		protected override void GoHome(object sender, RoutedEventArgs e)
		{
			if (Frame == null) return;
			while (Frame.CanGoBack) Frame.GoBack();
		}

        protected override void GoBack(object sender, RoutedEventArgs e)
		{
			if (Frame != null && Frame.CanGoBack) Frame.GoBack();
		}
	}
}