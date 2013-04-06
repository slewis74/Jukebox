using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Slew.WinRT.Pages.Settings;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.Requests;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Slew.WinRT.Pages.Navigation
{
    public class Navigator : INavigator
    {
        private readonly IPresentationBus _presentationBus;
        private readonly IControllerInvoker _controllerInvoker;
        private readonly int _settingsWidth;
        private Popup _settingsPopup;

        public Navigator(
            IPresentationBus presentationBus, 
            IControllerInvoker controllerInvoker)
        {
            _presentationBus = presentationBus;
            _controllerInvoker = controllerInvoker;

            _settingsWidth = 346;
        }

        public void Navigate<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController
        {
            var controllerResult = _controllerInvoker.Call(action);

            DoNavigate(controllerResult);
        }

        public async void Navigate<TController>(Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController
        {
            var controllerResult = await _controllerInvoker.CallAsync(action);

            DoNavigate(controllerResult);
        }

        public async void Navigate(string route)
        {
            var controllerResult = await _controllerInvoker.CallAsync(route);
            DoNavigate(controllerResult);
        }

        private void DoNavigate(ControllerInvokerResult controllerResult)
        {
            var route = controllerResult.Route;
            var result = controllerResult.Result;

            var settingsResult = result as ISettingsPageActionResult;
            if (settingsResult != null)
            {
                DoPopup(settingsResult);
                return;
            }

            var pageResult = result as IPageActionResult;
            if (pageResult != null)
            {
                _presentationBus.Publish(new PageNavigationRequest(route,
                                                                   new PageNavigationRequestEventArgs(pageResult.PageType,
                                                                                                      pageResult.Parameter)));
                return;
            }

            var viewModelResult = result as IViewModelActionResult;
            if (viewModelResult != null)
            {
                _presentationBus.Publish(new ViewModelNavigationRequest(route,
                                                                        new ViewModelNavigationRequestEventArgs(
                                                                            viewModelResult.ViewModelInstance)));
            }
        }

        public DataActionResult<TData> GetData<TController, TData>(
            Expression<Func<TController, ActionResult>> action)
            where TController : IController
        {
            var controllerResult = _controllerInvoker.Call(action);
            var result = controllerResult.Result;

            if ((result is DataActionResult<TData>) == false)
            {
                throw new InvalidOperationException("Controller action must return a DataActionResult when using GetData");
            }
            return (DataActionResult<TData>)result;
        }

        public async Task<DataActionResult<TData>> GetDataAsync<TController, TData>(
            Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController
        {
            var controllerResult = await _controllerInvoker.CallAsync(action);
            var result = controllerResult.Result;

            if ((result is DataActionResult<TData>) == false)
            {
                throw new InvalidOperationException("Controller action must return a DataActionResult when using GetData");
            }
            return (DataActionResult<TData>)result;
        }

        public void SettingsNavigateBack()
        {
            if (_settingsPopup != null)
            {
                _settingsPopup.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }

        private void DoPopup(ISettingsPageActionResult settingsResult)
        {
            var windowBounds = Window.Current.Bounds;

            // Create a Popup window which will contain our flyout.
            _settingsPopup = new Popup();
            _settingsPopup.Closed += OnPopupClosed;
            
            Window.Current.Activated += OnWindowActivated;
            
            _settingsPopup.IsLightDismissEnabled = true;

            _settingsPopup.Width = _settingsWidth;
            _settingsPopup.Height = windowBounds.Height;

            // Add the proper animation for the panel.
            _settingsPopup.ChildTransitions = new TransitionCollection
                                                  {
                                                      new PaneThemeTransition
                                                          {
                                                              Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right)
                                                                         ? EdgeTransitionLocation.Right
                                                                         : EdgeTransitionLocation.Left
                                                          }
                                                  };

            // Create a SettingsFlyout the same dimenssions as the Popup.
            var view = (FrameworkElement)Activator.CreateInstance(settingsResult.PageType);
            view.DataContext = settingsResult.Parameter;

            view.Width = _settingsWidth;
            view.Height = windowBounds.Height;

            // Place the SettingsFlyout inside our Popup window.
            var settingsPopupViewModel = new SettingsPopupViewModel(new SettingsBackCommand(this));

            _settingsPopup.Child = new SettingsPopupView
                                       {
                                           DataContext = settingsPopupViewModel,
                                           HeaderBackground = new SolidColorBrush(Colors.Green),
                                           Background = new SolidColorBrush(Colors.LightGreen),
                                           Content = view
                                       };

            // Let's define the location of our Popup.
            _settingsPopup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (windowBounds.Width - _settingsWidth) : 0);
            _settingsPopup.SetValue(Canvas.TopProperty, 0);
            _settingsPopup.IsOpen = true;
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
        }

    }
}