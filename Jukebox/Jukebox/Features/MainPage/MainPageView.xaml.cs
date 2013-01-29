﻿using System;
using System.Diagnostics;
using System.Threading;
using Jukebox.Features.MainPage.Events;
using Jukebox.Features.MainPage.Requests;
using Jukebox.Requests;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;
using Windows.Media;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Jukebox.Features.MainPage
{
    public sealed partial class MainPageView : 
        IHandlePresentationRequest<PlayFileRequest>,
        IHandlePresentationRequest<StopPlayingRequest>,
        IHandlePresentationRequest<PausePlayingRequest>,
        IHandlePresentationRequest<RestartPlayingRequest>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        
        public MainPageView()
		{
			InitializeComponent();
            
            SettingsPane.GetForCurrentView().CommandsRequested += MainPageCommandsRequested;

			Loaded += MainPageLoaded;
            Unloaded += OnUnloaded;

			MediaElement.MediaFailed += MediaElement_MediaFailed;

            MediaControl.PlayPressed += (sender, o) => DispatchCall(s => DoPlay());
		    MediaControl.PausePressed += (sender, o) => DispatchCall(s => DoPausePlaying());
		    MediaControl.PlayPauseTogglePressed += (sender, o) => DispatchCall(TogglePlayPause);
            MediaControl.StopPressed += (sender, o) => DispatchCall(s => DoStopPlaying());
            MediaControl.PreviousTrackPressed += (sender, o) => DispatchCall(s => PresentationBus.Publish(new PreviousTrackRequest()));
            MediaControl.NextTrackPressed += (sender, o) => DispatchCall(s => PresentationBus.Publish(new NextTrackRequest()));
		}

        public IPresentationBus PresentationBus { get; set; }
        public INavigator Navigator { get; set; }
        private MainPageViewModel ViewModel { get { return (MainPageViewModel)DataContext; } }
        public IViewResolver ViewResolver { get; set; }

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

        void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            NavFrame.ViewResolver = ViewResolver;
            PresentationBus.Subscribe(NavFrame);
            ViewModel.DisplayArtists.Execute(null);
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            PresentationBus.UnSubscribe(NavFrame);
        }

        void MainPageCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            //var viewType = BrowsingFrame.Content.GetType();
            //var request = new DisplaySettingsRequest(viewType, args.Request);
            //PresentationBus.Publish(request);
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
			Debug.WriteLine("MediaFailed: {0}", e.ErrorMessage);
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
	}
}
