using System.Linq;
using System.Windows.Input;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Features.MainPage.Commands;
using Jukebox.WinStore.Features.MainPage.Requests;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Requests;
using Slab.Data;
using Slab.PresentationBus;
using Slab.ViewModels;
using Slab.WinStore.Host;

namespace Jukebox.WinStore.Features.MainPage
{
	public class JukeboxHostViewModel :
        HostViewModel,
        IHandlePresentationRequest<PlayRequest>,
        IHandlePresentationRequest<PauseRequest>,
        IHandlePresentationRequest<StopRequest>,
        IHandlePresentationEvent<NowPlayingCurrentTrackChangedEvent>
	{
        private readonly DistinctAsyncObservableCollection<Playlist> _playlists;

        public delegate JukeboxHostViewModel Factory(
            DistinctAsyncObservableCollection<Playlist> playlists,
            NowPlayingPlaylist currentPlaylist);

        public JukeboxHostViewModel(
            IPresentationBus presentationBus,
            DistinctAsyncObservableCollection<Playlist> playlists,
            NowPlayingPlaylist currentPlaylist)
        {
            PresentationBus = presentationBus;
            NowPlayingPlaylist = currentPlaylist;
            _playlists = playlists;

            PlayCommand = new PresentationRequestCommand<PlayRequest>(PresentationBus);
            PauseCommand = new PresentationRequestCommand<PauseRequest>(PresentationBus);
            PlaylistsCommand = new PlaylistsCommand(_playlists);
            NextTrackCommand = new NextTrackCommand(PresentationBus, NowPlayingPlaylist.CanMoveNext);
            PreviousTrackCommand = new PreviousTrackCommand(PresentationBus, NowPlayingPlaylist.CanMovePrevious);
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
            if (e.Data != _nowPlayingPlaylist)
                return;

            var song = e.Song;

            if (song == null)
                StopPlaying();
            else
            {
                PlayFile(song);
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

	    private void AddToCurrentPlaylist(Artist artist)
		{
			foreach (var song in artist.Albums.SelectMany(a => a.Songs).OrderBy(s => s.Album.Title).ThenBy(s => s.TrackNumber))
			{
                NowPlayingPlaylist.Add(song);
            }
        }

        private void StartPlaying()
        {
            if (IsPlaying)
                return;

            if (IsPaused)
                RestartPlaying();
            else
            {
                PlayFile(NowPlayingPlaylist.CurrentTrack);
            }
        }
        
		private async void PlayFile(Song song)
		{
            await PresentationBus.PublishAsync(
                new PlayFileRequest(
                    song.Album.Artist.Name, 
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
	}
}