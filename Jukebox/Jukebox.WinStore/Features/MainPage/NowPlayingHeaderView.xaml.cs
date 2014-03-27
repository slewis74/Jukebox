using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Jukebox.WinStore.Features.MainPage.Events;
using Jukebox.WinStore.Features.MainPage.Requests;
using Jukebox.WinStore.Requests;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Features.MainPage
{
    public sealed partial class NowPlayingHeaderView :
        IHandlePresentationRequest<PlayFileRequest>,
        IHandlePresentationRequest<StopPlayingRequest>,
        IHandlePresentationRequest<PausePlayingRequest>,
        IHandlePresentationRequest<RestartPlayingRequest>
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private SystemMediaTransportControls _systemMediaTransportControls;

        public NowPlayingHeaderView()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            MediaElement.MediaFailed += MediaElementMediaFailed;
        }

        private JukeboxHostViewModel ViewModel { get { return (JukeboxHostViewModel) DataContext; } }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
            _systemMediaTransportControls.IsEnabled = true;
            _systemMediaTransportControls.ButtonPressed += (s, o) => DispatchCall(x => OnButtonPressed(o.Button));

            _systemMediaTransportControls.IsPlayEnabled = true;
            _systemMediaTransportControls.IsPauseEnabled = true;
            _systemMediaTransportControls.IsStopEnabled = true;

            if (ViewModel.NowPlayingPlaylist.CurrentTrack != null)
            {
                // Don't start playing, if Next/Previous are pressed first.
                MediaElement.AutoPlay = false;
                var storageFile = await ViewModel.NowPlayingPlaylist.CurrentTrack.GetStorageFileAsync();
                await SetupToPlay(storageFile);

                _systemMediaTransportControls.IsPreviousEnabled = ViewModel.NowPlayingPlaylist.CanMovePrevious;
                _systemMediaTransportControls.IsNextEnabled = ViewModel.NowPlayingPlaylist.CanMoveNext;
            }
        }

        private void OnButtonPressed(SystemMediaTransportControlsButton button)
        {
            switch (button)
            {
                case SystemMediaTransportControlsButton.Play:
                    TogglePlayPause();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    DoPausePlaying();
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    DoStopPlaying();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    ViewModel.PresentationBus.PublishAsync(new PreviousTrackRequest());
                    break;
                case SystemMediaTransportControlsButton.Next:
                    ViewModel.PresentationBus.PublishAsync(new NextTrackRequest());
                    break;

            }
        }

        void MediaElementMediaFailed(object sender, ExceptionRoutedEventArgs e)
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
            DoRestart();
        }

        private async Task SetupToPlay(StorageFile storageFile)
        {
            await _systemMediaTransportControls.DisplayUpdater.CopyFromFileAsync(MediaPlaybackType.Music, storageFile);
            _systemMediaTransportControls.DisplayUpdater.Update();

            // IMPORTANT NOTE: I left the following line in as a reminder/warning.  OpenReadAsync causes an issue with playback, after a couple of minutes
            //var stream = await storageFile.OpenReadAsync();
            var stream = await storageFile.OpenAsync(FileAccessMode.Read);
            MediaElement.SetSource(stream, storageFile.ContentType);
        }

        private async void DoPlay(StorageFile storageFile)
        {
            await SetupToPlay(storageFile);
            MediaElement.Play();
        }

        private void TogglePlayPause()
        {
            if (MediaElement.CurrentState == MediaElementState.Playing)
            {
                DoPausePlaying();
            }
            else
            {
                DoRestart();
            }
        }

        private void DoRestart()
        {
            MediaElement.AutoPlay = true;
            MediaElement.Play();
            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
            ViewModel.IsPlaying = true;
        }

        private void DoStopPlaying()
        {
            MediaElement.Stop();
            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
            ViewModel.IsPlaying = false;
            ViewModel.IsPaused = false;
        }

        private void DoPausePlaying()
        {
            MediaElement.AutoPlay = false;
            MediaElement.Pause();
            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
            ViewModel.IsPaused = true;
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
            ViewModel.PresentationBus.PublishAsync(new SongEndedEvent());
        }
    }
}
