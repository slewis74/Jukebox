using System;
using System.Diagnostics;
using System.Threading;
using Jukebox.Features.MainPage.Events;
using Jukebox.Features.MainPage.Requests;
using Jukebox.Requests;
using Slew.WinRT.Container;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.Requests;
using Windows.Foundation;
using Windows.Media;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Jukebox.Features.MainPage
{
    public sealed partial class MainPage : 
        IPublish,
        IHandlePresentationRequest<NavigationRequest>,
        IHandlePresentationRequest<PositionTransformRequest>,
        IHandlePresentationRequest<PlayFileRequest>,
        IHandlePresentationRequest<StopPlayingRequest>,
        IHandlePresentationRequest<PausePlayingRequest>,
        IHandlePresentationRequest<RestartPlayingRequest>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        
        public MainPage()
		{
			InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += MainPageCommandsRequested;

            PropertyInjector.Inject(() => NowPlayingView);

			Loaded += MainPageLoaded;
			MediaElement.MediaFailed += MediaElement_MediaFailed;
            MediaControl.PlayPressed += (sender, o) => DispatchCall(s => DoPlay());
		    MediaControl.PausePressed += (sender, o) => DispatchCall(s => DoPausePlaying());
		    MediaControl.PlayPauseTogglePressed += (sender, o) => DispatchCall(TogglePlayPause);
            MediaControl.StopPressed += (sender, o) => DispatchCall(s => DoStopPlaying());
            MediaControl.PreviousTrackPressed += (sender, o) => DispatchCall(s => PresentationBus.Publish(new PreviousTrackRequest()));
            MediaControl.NextTrackPressed += (sender, o) => DispatchCall(s => PresentationBus.Publish(new NextTrackRequest()));

            BrowsingFrame.Navigated += (sender, args) => PropertyInjector.Inject(() => args.Content);
		}

        public IPresentationBus PresentationBus { get; set; }
        private MainPageViewModel ViewModel { get { return (MainPageViewModel)DataContext; } }

        private void DispatchCall(SendOrPostCallback call)
        {
            if (SynchronizationContext.Current != _synchronizationContext)
            {
                _synchronizationContext.Post(call, null);
            }
            else
            {
                call(null);
            }
        }

        void MainPageCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("ShuffleMode", "Shuffle", command => { }));
            args.Request.ApplicationCommands.Add(new SettingsCommand("ManagePlaylists", "Playlists", command => DoManagePlaylists()));
        }

        private void DoManagePlaylists()
        {
            
        }

        private void TogglePlayPause(object state)
        {
            if (MediaElement.CurrentState == MediaElementState.Paused)
            {
                MediaElement.Play();
                ViewModel.IsPlaying = true;
            }
            else
            {
                MediaElement.Pause();
                ViewModel.IsPlaying = false;
            }
        }

		void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			Debug.WriteLine(string.Format("MediaFailed: {0}", e.ErrorMessage));
		}

		void MainPageLoaded(object sender, RoutedEventArgs e)
		{
		    ViewModel.DisplayArtists.Execute(null);
		}

        public void Handle(NavigationRequest request)
        {
            BrowsingFrame.Navigate(request.Args.ViewType, request.Args.Parameter);
        }

        public void Handle(PlayFileRequest request)
        {
            DoPlay(request.StorageFile);
        }

        public void Handle(StopPlayingRequest request)
        {
            DoStopPlaying();
        }
        public void Handle(PausePlayingRequest request)
        {
            DoPausePlaying();
        }
        public void Handle(RestartPlayingRequest request)
        {
            DoPlay();
        }

		protected override void GoBack(object sender, RoutedEventArgs e)
		{
			// Use the navigation frame to return to the previous page
			if (BrowsingFrame != null && BrowsingFrame.CanGoBack) BrowsingFrame.GoBack();
		}

		private async void DoPlay(StorageFile storageFile)
		{
			//var stream = await storageFile.OpenReadAsync();
			var stream = await storageFile.OpenAsync(FileAccessMode.Read);

			MediaElement.SetSource(stream, storageFile.FileType);
		    DoPlay();
		}
        private void DoPlay()
        {
            MediaElement.Play();
        }

        private void DoStopPlaying()
        {
            MediaElement.Stop();
        }
        private void DoPausePlaying()
        {
            MediaElement.Pause();
        }

		private void MediaElementMediaEnded1(object sender, RoutedEventArgs e)
		{
            PresentationBus.Publish(new SongEndedEvent());
		}

		private void BrowsingFrameNavigated1(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			var view = e.Content as LayoutAwarePageWithNavigation;
			if (view != null)
			{
				view.ProvideAppBarContent += ViewOnProvideAppBarContent;
			}
		}

		private void BrowsingFrameNavigating1(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
		{
			var currentView = ((Frame)sender).Content as LayoutAwarePageWithNavigation;
			if (currentView != null)
			{
				currentView.ProvideAppBarContent -= ViewOnProvideAppBarContent;
			}
		}

		private void ViewOnProvideAppBarContent(object sender, FrameworkElement frameworkElement)
		{
			PageCommands.Children.Clear();
            
            if (frameworkElement == null) return;
			PageCommands.Children.Add(frameworkElement);
		}

        public void Handle(PositionTransformRequest request)
        {
            var element = request.Args;

            var position = element.TransformToVisual(this).TransformPoint(new Point(0, 0));

            var location = new Location
            {
                Position = position,
                Size = new Size(element.ActualWidth, element.ActualHeight)
            };

            request.Location = location;
            request.IsHandled = true;
        }
	}
}
