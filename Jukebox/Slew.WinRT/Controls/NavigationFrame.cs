using System;
using System.Collections.Generic;
using System.Linq;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.Requests;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Slew.WinRT.Controls
{
    public class NavigationFrame : 
        ContentControl,
        IHandlePresentationEvent<PageNavigationRequest>,
        IHandlePresentationEvent<ViewModelNavigationRequest>
    {
        private readonly Stack<FrameworkElement> _backStack;

        public NavigationFrame()
        {
            _backStack = new Stack<FrameworkElement>();
        }

        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof (string), typeof (NavigationFrame), new PropertyMetadata(default(string)));

        public string TargetName
        {
            get { return (string) GetValue(TargetNameProperty); }
            set { SetValue(TargetNameProperty, value); }
        }

        public static readonly DependencyProperty ViewLocatorProperty =
            DependencyProperty.Register("ViewLocator", typeof (IViewLocator), typeof (NavigationFrame), new PropertyMetadata(default(IViewLocator)));

        public IViewLocator ViewLocator
        {
            get { return (IViewLocator)GetValue(ViewLocatorProperty); }
            set { SetValue(ViewLocatorProperty, value); }
        }

        public static readonly DependencyProperty PageCommandsPanelProperty =
            DependencyProperty.Register("PageCommandsPanel", typeof (Panel), typeof (NavigationFrame), new PropertyMetadata(default(Panel)));

        public Panel PageCommandsPanel
        {
            get { return (Panel) GetValue(PageCommandsPanelProperty); }
            set { SetValue(PageCommandsPanelProperty, value); }
        }

        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register("CanGoBack", typeof (bool), typeof (NavigationFrame), new PropertyMetadata(default(bool)));

        public bool CanGoBack
        {
            get { return (bool) GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }

        public void Handle(PageNavigationRequest request)
        {
            if (request.Args.Target != TargetName)
                return;

            // create the view instance.
            var view = (FrameworkElement)Activator.CreateInstance(request.Args.ViewType);
            view.DataContext = request.Args.Parameter;

            if (Content != null)
            {
                _backStack.Push((FrameworkElement)Content);
            }

            Content = view;
            SetCanGoBack();
        }

        public void Handle(ViewModelNavigationRequest request)
        {
            if (request.Args.Target != TargetName)
                return;

            var view = ViewLocator.Resolve(request.Args.ViewModel, ApplicationView.Value);

            var contentSwitchingPage = new ContentSwitchingPage
                                           {
                                               DataContext = request.Args.ViewModel,
                                               ViewLocator = ViewLocator
                                           };

            PageCommandsPanel.Children.Clear();
            var hasAppBarContent = view as IHaveBottomAppBar;
            if (hasAppBarContent != null)
            {
                var frameworkElement = (FrameworkElement)Activator.CreateInstance(hasAppBarContent.BottomAppBarContentType);
                frameworkElement.DataContext = request.Args.ViewModel;

                PageCommandsPanel.Children.Add(frameworkElement);
            }

            if (Content != null)
            {
                _backStack.Push((FrameworkElement)Content);
            }

            Content = contentSwitchingPage;
            SetCanGoBack();
        }

        private void SetCanGoBack()
        {
            CanGoBack = _backStack.Any();
        }

        public void GoBack()
        {
            if (CanGoBack == false)
                return;

            var view = _backStack.Pop();
            Content = view;
            SetCanGoBack();
        }
    }
}