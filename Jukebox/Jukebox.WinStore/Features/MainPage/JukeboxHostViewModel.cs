using System.Windows.Input;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Features.MainPage.Commands;
using Jukebox.WinStore.Features.MainPage.Requests;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Requests;
using Slab.Data;
using Slew.PresentationBus;
using Slab.ViewModels;
using Slab.WinStore.Host;

namespace Jukebox.WinStore.Features.MainPage
{
	public class JukeboxHostViewModel :
        HostViewModel,
        IHandlePresentationRequest<PlayRequest>,
        IHandlePresentationRequest<PauseRequest>,
        IHandlePresentationRequest<StopRequest>,
        IHandlePresentationEvent<NowPlayingCurrentTrackChangedEvent>,
        IHandlePresentationEvent<PlaylistDataLoaded>
	{
        private readonly DistinctAsyncObservableCollection<Playlist> _playlists;

        public JukeboxHostViewModel(
            IPresentationBus presentationBus,
            NextTrackCommand nextTrackCommand,
            PreviousTrackCommand previousTrackCommand)
        {
            PresentationBus = presentationBus;

            _playlists = new DistinctAsyncObservableCollection<Playlist>();
            _nowPlayingPlaylist = new NowPlayingPlaylist(presentationBus, false);

            PlayCommand = new PresentationRequestCommand<PlayRequest>(PresentationBus);
            PauseCommand = new PresentationRequestCommand<PauseRequest>(PresentationBus);
            PlaylistsCommand = new PlaylistsCommand(_playlists);
            NextTrackCommand = nextTrackCommand;
            PreviousTrackCommand = previousTrackCommand;
        }
        
        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public ICommand PlaylistsCommand { get; private set; }
        public ICommand NextTrackCommand { get; private set; }
        public ICommand PreviousTrackCommand { get; private set; }

        public DistinctAsyncObservableCollection<Playlist> Playlists { get { return _playlists; }}

        private NowPlayingPlaylist _nowPlayingPlaylist;
	    public NowPlayingPlaylist NowPlayingPlaylist
	    {
	        get { return _nowPlayingPlaylist; }
	        set
	        {
	            SetProperty(ref _nowPlayingPlaylist, value);
	        }
	    }

	    private bool _isPaused;
	    public bool IsPaused
	    {
	        get { return _isPaused; }
	        set
	        {
                if (SetProperty(ref _isPaused, value) && value)
	            {
	                IsPlaying = false;
	            }
	        }
	    }

	    private bool _isPlaying;
	    public bool IsPlaying
	    {
	        get { return _isPlaying; }
	        set
	        {
                if (SetProperty(ref _isPlaying, value))
                {
                    if (_isPlaying)
                    {
                        IsPaused = false;
                    }
                    NotifyChanged(() => IsNotPlaying);
                }
	        }
	    }

	    public bool IsNotPlaying
	    {
	        get { return !IsPlaying; }
	        set { IsPlaying = !value; }
	    }

        private string _currentTrackDescription;
	    public string CurrentTrackDescription
        {
            get { return _currentTrackDescription; }
            set
            {
                SetProperty(ref _currentTrackDescription, value);
            }
        }

        public void Handle(NowPlayingCurrentTrackChangedEvent e)
        {
            if (e.Playlist != _nowPlayingPlaylist)
                return;

            var song = e.PlaylistSong;

            if (song == null)
                StopPlaying();
            else
            {
                PlayFile(e.PlaylistSong.ArtistName, e.PlaylistSong.Song);
            }
        }

        public void Handle(PlayRequest request)
        {
            StartPlaying();
        }

        public void Handle(PauseRequest request)
        {
            if (IsNotPlaying)
                return;
            PausePlaying();
        }
        public void Handle(StopRequest request)
        {
            if (IsNotPlaying)
                return;
            StopPlaying();
        }
        
        private void StartPlaying()
        {
            if (IsPlaying)
                return;

            if (IsPaused)
                RestartPlaying();
            else
            {
                PlayFile(NowPlayingPlaylist.CurrentTrack.ArtistName, NowPlayingPlaylist.CurrentTrack.Song);
            }
        }
        
		private async void PlayFile(string artistName, Song song)
		{
            await PresentationBus.PublishAsync(
                new PlayFileRequest(
                    artistName, 
                    song.Title, 
                    await song.GetStorageFileAsync()));
		}

	    private void RestartPlaying()
        {
            PresentationBus.PublishAsync(new RestartPlayingRequest());
        }

        private void PausePlaying()
        {
            PresentationBus.PublishAsync(new PausePlayingRequest());
        }

		private void StopPlaying()
		{
            PresentationBus.PublishAsync(new StopPlayingRequest());
		}

	    public void Handle(PlaylistDataLoaded presentationEvent)
	    {
	        Playlists.StartLargeUpdate();
            Playlists.AddRange(presentationEvent.PlaylistData.Playlists);
            Playlists.CompleteLargeUpdate();

	        NowPlayingPlaylist = presentationEvent.PlaylistData.NowPlayingPlaylist;
	    }
	}
}