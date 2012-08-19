using Slew.WinRT.Pages;

namespace Slew.WinRT.ViewModels
{
	public interface ICanRequestNavigation
	{
        INavigator Navigator { get; set; }
	}
}