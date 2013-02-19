using System;
using System.Collections.Generic;
using System.Linq;
using Slew.WinRT.Data.Navigation;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;
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
            public FrameworkElement Content { get; set; }

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

        public static readonly DependencyProperty NavigationStackStorageProperty =
            DependencyProperty.Register("NavigationStackStorage", typeof(INavigationStackStorage), typeof(NavigationFrame), new PropertyMetadata(default(INavigationStackStorage)));

        public INavigationStackStorage NavigationStackStorage
        {
            get { return (INavigationStackStorage)GetValue(NavigationStackStorageProperty); }
            set { SetValue(NavigationStackStorageProperty, value); }
        }

        public static readonly DependencyProperty DefaultUriProperty =
            DependencyProperty.Register("DefaultUri", typeof (string), typeof (NavigationFrame), new PropertyMetadata(default(string)));

        public string DefaultUri
        {
            get { return (string) GetValue(DefaultUriProperty); }
            set { SetValue(DefaultUriProperty, value); }
        }

        public static readonly DependencyProperty ControllerInvokerProperty =
            DependencyProperty.Register("ControllerInvoker", typeof(IControllerInvoker), typeof(NavigationFrame), new PropertyMetadata(default(IControllerInvoker)));

        public IControllerInvoker ControllerInvoker
        {
            get { return (IControllerInvoker)GetValue(ControllerInvokerProperty); }
            set { SetValue(ControllerInvokerProperty, value); }
        }

        public void RestoreNavigationStack()
        {
            if (NavigationStackStorage == null)
                return;

            var uris = NavigationStackStorage.RetrieveUris();

            NavigationFrameStackItem navigationFrameStackItem;
            if (uris == null || uris.Any() == false)
            {
                navigationFrameStackItem = new NavigationFrameStackItem(DefaultUri, null);
                _navigationStack.Push(navigationFrameStackItem);
            }
            else
            {
                foreach (var uri in uris)
                {
                    _navigationStack.Push(new NavigationFrameStackItem(uri, null));
                }
                navigationFrameStackItem = _navigationStack.Peek();
            }

            CheckItemContent(navigationFrameStackItem);
            Content = navigationFrameStackItem.Content;

            SetCanGoBack();
        }

        public void Handle(PageNavigationRequest request)
        {
            if (request.Args.Target != TargetName)
                return;

            NavigateToPage(request.Uri, request.Args.ViewType, request.Args.Parameter);
        }

        private void NavigateToPage(string uri, Type viewType, object parameter)
        {
            // create the view instance.
            var view = (FrameworkElement)Activator.CreateInstance(viewType);
            view.DataContext = parameter;

            GoForward(uri, view);
        }

        public void Handle(ViewModelNavigationRequest request)
        {
            if (request.Args.Target != TargetName)
                return;

            NavigateToViewModelAndAddToStack(request.Uri, request.Args.ViewModel);
        }

        private void NavigateToViewModelAndAddToStack(string uri, object viewModel)
        {
            var contentSwitchingPage = NavigateToViewModel(viewModel);

            GoForward(uri, contentSwitchingPage);
        }

        private FrameworkElement NavigateToViewModel(object viewModel)
        {
            var view = ViewLocator.Resolve(viewModel, ApplicationView.Value);

            var contentSwitchingPage = new ContentSwitchingPage
                                           {
                                               DataContext = viewModel,
                                               ViewLocator = ViewLocator
                                           };

            PageCommandsPanel.Children.Clear();
            var hasAppBarContent = view as IHaveBottomAppBar;
            if (hasAppBarContent != null)
            {
                var frameworkElement = (FrameworkElement) Activator.CreateInstance(hasAppBarContent.BottomAppBarContentType);
                frameworkElement.DataContext = viewModel;

                PageCommandsPanel.Children.Add(frameworkElement);
            }
            return contentSwitchingPage;
        }

        private void GoForward(string uri, FrameworkElement newContent)
        {
            _navigationStack.Push(new NavigationFrameStackItem(uri, newContent));
            Content = newContent;
            SetCanGoBack();

            if (NavigationStackStorage != null)
            {
                NavigationStackStorage.StoreUris(_navigationStack.Select(i => i.Uri).ToArray());
            }
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
            CheckItemContent(item);
            Content = item.Content;
            SetCanGoBack();

            if (NavigationStackStorage != null)
            {
                NavigationStackStorage.StoreUris(_navigationStack.Select(i => i.Uri).ToArray());
            }
        }

        private void CheckItemContent(NavigationFrameStackItem item)
        {
            if (item.Content != null)
                return;

            var controllerResult = ControllerInvoker.Call(item.Uri);
            if (controllerResult is IPageActionResult)
            {
                //NavigateToPage();
            }
            else if (controllerResult is ViewModelActionResult)
            {
                var view = NavigateToViewModel(((ViewModelActionResult)controllerResult).ViewModelInstance);
                item.Content = view;
            }
        }
    }
}