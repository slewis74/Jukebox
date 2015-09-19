using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Jukebox.WinStore.Features.MainPage.Events;
using Jukebox.WinStore.Features.MainPage.Requests;
using Jukebox.WinStore.Requests;
using Orienteer.WinStore.Pages;
using PresentationBus;

namespace Jukebox.WinStore.Features.MainPage
{
    public sealed partial class NowPlayingHeaderView :
        IHandlePresentationCommand<PlayFileCommand>,
        IHandlePresentationCommand<StopPlayingCommand>,
        IHandlePresentationCommand<PausePlayingCommand>,
        IHandlePresentationCommand<RestartPlayingCommand>,
        IHandlePresentationRequest<PositionTransformRequest, PositionTransformResponse>
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
                var storageFile = await ViewModel.NowPlayingPlaylist.CurrentTrack.Song.GetStorageFileAsync();
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
                    ViewModel.PresentationBus.Send(new PreviousTrackCommand());
                    break;
                case SystemMediaTransportControlsButton.Next:
                    ViewModel.PresentationBus.Send(new NextTrackCommand());
                    break;

            }
        }

        void MediaElementMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("MediaFailed: {0}", e.ErrorMessage);
        }

        public void Handle(PlayFileCommand command)
        {
            DoPlay(command.StorageFile);
        }

        public void Handle(StopPlayingCommand command)
        {
            DoStopPlaying();
        }
        public void Handle(PausePlayingCommand command)
        {
            DoPausePlaying();
        }
        public void Handle(RestartPlayingCommand command)
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
            DoRestart();
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
            ViewModel.PresentationBus.Publish(new SongEndedEvent());
        }

        public PositionTransformResponse Handle(PositionTransformRequest request)
        {
            var element = request.Element;

            var position = element.TransformToVisual((UIElement)this.Parent).TransformPoint(new Point(0, 0));

            var location = new Location
            {
                Position = position,
                Size = new Size(element.ActualWidth, element.ActualHeight)
            };

            return new PositionTransformResponse { Location = location };
        }
    }
}
