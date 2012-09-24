﻿using System;
using System.Diagnostics;
using System.Threading;
using Jukebox.MainPage.Events;
using Jukebox.MainPage.Requests;
using Jukebox.Requests;
using Slew.WinRT.Container;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.Requests;
using Windows.Foundation;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Jukebox.MainPage
{
    public sealed partial class MainPage : 
        IPublish,
        IHandlePresentationRequest<NavigationRequest>,
        IHandlePresentationRequest<PlayFileRequest>,
        IHandlePresentationRequest<StopPlayingRequest>,
        IHandlePresentationRequest<PausePlayingRequest>,
        IHandlePresentationRequest<RestartPlayingRequest>,
        IHandlePresentationRequest<PlayDropLocationRequest>,
        IHandlePresentationRequest<PlaylistDropLocationRequest>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        
        public MainPage()
		{
			InitializeComponent();
			Loaded += MainPageLoaded;
			MediaElement.MediaFailed += MediaElement_MediaFailed;
            MediaControl.PlayPressed += (sender, o) => DispatchCall(s => DoPlay());
		    MediaControl.PausePressed += (sender, o) => DispatchCall(s => DoPausePlaying());
		    MediaControl.PlayPauseTogglePressed += (sender, o) => DispatchCall(TogglePlayPause);
            MediaControl.StopPressed += (sender, o) => DispatchCall(s => DoStopPlaying());
            MediaControl.PreviousTrackPressed += (sender, o) => DispatchCall(s => PresentationBus.Publish(new PreviousTrackRequest()));
            MediaControl.NextTrackPressed += (sender, o) => DispatchCall(s => PresentationBus.Publish(new NextTrackRequest()));

            BrowsingFrame.Navigated += (sender, args) => PropertyInjector.Resolve(() => args.Content);
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

        public void Handle(PlayDropLocationRequest request)
        {
            request.IsHandled = true;
            request.Location = GetPlayDropLocation();
        }

        private Location GetPlayDropLocation()
        {
            var position = playPauseGrid.TransformToVisual(this).TransformPoint(new Point(0, 0));

            var location = new Location
                               {
                                   Position = position,
                                   Size = new Size(playPauseGrid.ActualWidth, playPauseGrid.ActualHeight)
                               };

            return location;
        }

        public void Handle(PlaylistDropLocationRequest request)
        {
            request.IsHandled = true;
            request.Location = GetPlaylistDropLocation();
        }

        private Location GetPlaylistDropLocation()
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
}
