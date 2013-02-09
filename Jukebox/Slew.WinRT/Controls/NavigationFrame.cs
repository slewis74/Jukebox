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
        private class NavigationFrameStackItem
        {
            public NavigationFrameStackItem(string uri, FrameworkElement content)
            {
                Uri = uri;
                Content = content;
            }

            public string Uri { get; private set; }
            public FrameworkElement Content { get; private set; }

            public override bool Equals(object obj)
            {
                return obj is NavigationFrameStackItem && ((NavigationFrameStackItem) obj).Uri == Uri;
            }
            public override int GetHashCode()
            {
                return Uri.GetHashCode();
            }
        }

        private readonly Stack<NavigationFrameStackItem> _navigationStack;

        public NavigationFrame()
        {
            // Note: the top of the navigation stack is always the currently displayed page
            _navigationStack = new Stack<NavigationFrameStackItem>();
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

            GoForward(request.Uri, view);
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

            GoForward(request.Uri, contentSwitchingPage);
        }

        private void GoForward(string uri, FrameworkElement newContent)
        {
            _navigationStack.Push(new NavigationFrameStackItem(uri, newContent));
            Content = newContent;
            SetCanGoBack();
        }

        private void SetCanGoBack()
        {
            CanGoBack = _navigationStack.Count() > 1;
        }

        public void GoBack()
        {
            if (CanGoBack == false)
                return;
            
            // pop the current page off the stack
            _navigationStack.Pop();


            var item = _navigationStack.Peek();
            Content = item.Content;
            SetCanGoBack();
        }
    }
}