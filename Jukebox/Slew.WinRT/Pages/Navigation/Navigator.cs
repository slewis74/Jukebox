﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Slew.WinRT.Container;
using Slew.WinRT.Pages.Settings;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.Requests;
using Slew.WinRT.ViewModels;
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
        private readonly IControllerFactory _controllerFactory;
        private Popup _settingsPopup;
        private int _settingsWidth;

        public Navigator(
            IPresentationBus presentationBus, 
            IControllerFactory controllerFactory)
        {
            _presentationBus = presentationBus;
            _controllerFactory = controllerFactory;

            _settingsWidth = 346;
        }

        public void Navigate<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController, new()
        {
            var instance = _controllerFactory.Create<TController>();

            var body = (MethodCallExpression)action.Body;
            var parameterValues = new List<object>();
            var parameters = body.Method.GetParameters();
            var arguments = body.Arguments;
            for (var i = 0; i < parameters.Length && i < arguments.Count; i++)
            {
                var argument = arguments[i];
                var lambda = Expression.Lambda<Func<TController, object>>(Expression.Convert(argument, typeof(object)), action.Parameters.ToList());
                var compiled = lambda.Compile();
                var value = compiled(default(TController));
                
                parameterValues.Add(value);
            }

            var result = (ActionResult)body.Method.Invoke(instance, parameterValues.ToArray());

            var settingsResult = result as ISettingsPageActionResult;
            if (settingsResult != null)
            {
                DoPopup(settingsResult);
                return;
            }

            var pageResult = result as IPageActionResult;
            if (pageResult != null)
            {
                var canRequestNavigation = pageResult.Parameter as ICanRequestNavigation;
                if (canRequestNavigation != null)
                {
                    canRequestNavigation.Navigator = this;
                }

                _presentationBus.Publish(new NavigationRequest(new NavigationRequestEventArgs(pageResult.PageType, pageResult.Parameter)));
                return;
            }

            var viewModelResult = result as IViewModelActionResult;
            if (viewModelResult != null)
            {
                var canRequestNavigation = viewModelResult.ViewModelInstance as ICanRequestNavigation;
                if (canRequestNavigation != null)
                {
                    canRequestNavigation.Navigator = this;
                }

                var viewModelWithOrientation = viewModelResult.ViewModelInstance as ViewModelWithOrientation;
                if (viewModelWithOrientation == null)
                {
                    throw new InvalidOperationException("When navigating to a view model it must be a ViewModelWithOrientation");
                }

                _presentationBus.Publish(new NavigationRequest(new NavigationRequestEventArgs(typeof(ContentSwitchingPage), viewModelResult.ViewModelInstance)));
            }
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
            _settingsPopup.Child = new SettingsPopupView
                                       {
                                           DataContext = new SettingsPopupViewModel(),
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