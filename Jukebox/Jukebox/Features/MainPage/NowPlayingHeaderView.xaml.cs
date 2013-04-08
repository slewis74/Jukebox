using System;
using System.Diagnostics;
using System.Threading;
using Jukebox.Features.MainPage.Events;
using Jukebox.Features.MainPage.Requests;
using Jukebox.Requests;
using Slab.PresentationBus;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Jukebox.Features.MainPage
{
    public sealed partial class NowPlayingHeaderView :
        IHandlePresentationRequest<PlayFileRequest>,
        IHandlePresentationRequest<StopPlayingRequest>,
        IHandlePresentationRequest<PausePlayingRequest>,
        IHandlePresentationRequest<RestartPlayingRequest>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        public NowPlayingHeaderView()
        {
            InitializeComponent();

            MediaElement.MediaFailed += MediaElementMediaFailed;

            MediaControl.PlayPressed += (sender, o) => DispatchCall(s => DoPlay());
            MediaControl.PausePressed += (sender, o) => DispatchCall(s => DoPausePlaying());
            MediaControl.PlayPauseTogglePressed += (sender, o) => DispatchCall(TogglePlayPause);
            MediaControl.StopPressed += (sender, o) => DispatchCall(s => DoStopPlaying());
            MediaControl.PreviousTrackPressed += (sender, o) => DispatchCall(s => ViewModel.PresentationBus.Publish(new PreviousTrackRequest()));
            MediaControl.NextTrackPressed += (sender, o) => DispatchCall(s => ViewModel.PresentationBus.Publish(new NextTrackRequest()));

        }

        public JukeboxHostViewModel ViewModel { get { return ((JukeboxHostViewModel)DataContext); } }

        void MediaElementMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("MediaFailed: {0}", e.ErrorMessage);
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
        public void Handle(PlayFileRequest request)
        {
            DoPlay(request.ArtistName, request.TrackTitle, request.StorageFile);
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

        private async void DoPlay(string artistName, string trackTitle, StorageFile storageFile)
        {
            //var stream = await storageFile.OpenReadAsync();
            var stream = await storageFile.OpenAsync(FileAccessMode.Read);

            MediaControl.ArtistName = artistName;
            MediaControl.TrackName = trackTitle;
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

        private void MediaElementMediaEnded1(object sender, RoutedEventArgs e)
        {
            ViewModel.PresentationBus.Publish(new SongEndedEvent());
        }
    }
}
