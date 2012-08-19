using System;
using System.Diagnostics;
using System.Threading;
using Slew.WinRT.Pages;
using Windows.Foundation;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Jukebox
{
    public sealed partial class MainPage : IAcceptPlaylistDragging
	{
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        
        public MainPage()
		{
			InitializeComponent();
			Loaded += MainPageLoaded;
			MediaElement.MediaFailed += MediaElement_MediaFailed;
            MediaControl.PlayPressed += (sender, o) => DispatchCall(s => MediaElement.Play());
		    MediaControl.PausePressed += (sender, o) => DispatchCall(s => MediaElement.Pause());
		    MediaControl.PlayPauseTogglePressed += (sender, o) => DispatchCall(TogglePlayPause);
            MediaControl.StopPressed += (sender, o) => DispatchCall(s => MediaElement.Stop());
            MediaControl.PreviousTrackPressed += (sender, o) => DispatchCall(s => ViewModel.PreviousTrack());
            MediaControl.NextTrackPressed += (sender, o) => DispatchCall(s => ViewModel.NextTrack());
		}

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

        private void TogglePlayPause(object state)
        {
            if (MediaElement.CurrentState == MediaElementState.Paused)
                MediaElement.Play();
            else
                MediaElement.Pause();
        }

		void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			Debug.WriteLine(string.Format("MediaFailed: {0}", e.ErrorMessage));
		}

        private MainPageViewModel ViewModel { get { return (MainPageViewModel) DataContext; } }

		void MainPageLoaded(object sender, RoutedEventArgs e)
		{
            var mainPageViewModel = ViewModel;

			mainPageViewModel.NavigateRequest += (s, e1) => BrowsingFrame.Navigate(e1.ViewType, e1.Parameter);

			mainPageViewModel.DisplayArtists.Execute(null);

			mainPageViewModel.StopPlaying += (s, _) => MediaElement.Stop();
			mainPageViewModel.RestartPlaying += (s, _) => MediaElement.Play();
			mainPageViewModel.PausePlaying += (s, _) => MediaElement.Pause();
			mainPageViewModel.PlayFile += (s, storageFile) => DoPlay(storageFile);
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
			MediaElement.Play();
        }

		private void MediaElementMediaEnded1(object sender, RoutedEventArgs e)
		{
			var mainPageViewModel = (MainPageViewModel)DataContext;
			mainPageViewModel.SongFinished();
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

        public Location GetPlayDropLocation()
        {
            var position = playPauseGrid.TransformToVisual(this).TransformPoint(new Point(0, 0));

            var location = new Location
                               {
                                   Position = position,
                                   Size = new Size(playPauseGrid.ActualWidth, playPauseGrid.ActualHeight)
                               };

            return location;
        }

        public Location GetPlaylistDropLocation()
        {
            var position = playlistControl.TransformToVisual(this).TransformPoint(new Point(0, 0));

            var location = new Location
                               {
                                   Position = position,
                                   Size = new Size(playlistControl.ActualWidth, playlistControl.ActualHeight)
                               };

            return location;
        }
	}

    public interface IAcceptPlaylistDragging
    {
        Location GetPlayDropLocation();
        Location GetPlaylistDropLocation();
    }
}
