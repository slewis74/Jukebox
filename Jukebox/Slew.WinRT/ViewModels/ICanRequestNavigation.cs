using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.ViewModels
{
	public interface ICanRequestNavigation
	{
        INavigator Navigator { get; set; }
	}
}