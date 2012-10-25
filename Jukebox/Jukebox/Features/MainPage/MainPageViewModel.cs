using System.Linq;
using System.Windows.Input;
using Jukebox.Events;
using Jukebox.Features.MainPage.Commands;
using Jukebox.Features.MainPage.Requests;
using Jukebox.Model;
using Jukebox.Requests;
using Slew.WinRT.Container;
using Slew.WinRT.Data;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.MainPage
{
	public class MainPageViewModel :
        CanRequestNavigationBase,
        IPublish,
        IHandlePresentationRequest<PlayRequest>,
        IHandlePresentationRequest<PauseRequest>,
        IHandlePresentationRequest<StopRequest>,
        IHandlePresentationEvent<NowPlayingCurrentTrackChangedEvent>
	{
        private readonly DistinctAsyncObservableCollection<Playlist> _playlists;

        public MainPageViewModel(
            DistinctAsyncObservableCollection<Artist> artists,
            DistinctAsyncObservableCollection<Playlist> playlists,
            NowPlayingPlaylist currentPlaylist)
		{
            NowPlayingPlaylist = currentPlaylist;
            _artists = artists;
            _playlists = playlists;

			DisplayArtists = PropertyInjector.Inject(() => new DisplayArtistsCommand(_artists));
            
            PlayCommand = PropertyInjector.Inject(() => new PresentationRequestCommand<PlayRequest>());
            PauseCommand = PropertyInjector.Inject(() => new PresentationRequestCommand<PauseRequest>());
            PlaylistsCommand = PropertyInjector.Inject(() => new PlaylistsCommand(_playlists));
            NextTrackCommand = PropertyInjector.Inject(() => new NextTrackCommand(NowPlayingPlaylist.CanMoveNext));
            PreviousTrackCommand = PropertyInjector.Inject(() => new PreviousTrackCommand(NowPlayingPlaylist.CanMovePrevious));
        }

        public IPresentationBus PresentationBus { get; set; }

		public DisplayArtistsCommand DisplayArtists { get; private set; }

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

	    private readonly DistinctAsyncObservableCollection<Artist> _artists;
        public DistinctAsyncObservableCollection<Artist> Artists { get { return _artists; } }

	    private bool _isPaused;
	    public bool IsPaused
	    {
	        get { return _isPaused; }
	        set
	        {
	            SetProperty(ref _isPaused, value);
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
            IsPaused = false;
            IsPlaying = true;
		    PresentationBus.Publish(new PlayFileRequest(await song.GetStorageFileAsync()));
		}

	    private void RestartPlaying()
        {
            IsPlaying = true;
            IsPaused = false;
            PresentationBus.Publish(new RestartPlayingRequest());
        }

        private void PausePlaying()
        {
            IsPlaying = false;
            IsPaused = true;
            PresentationBus.Publish(new PausePlayingRequest());
        }

		private void StopPlaying()
		{
		    IsPlaying = false;
		    IsPaused = false;
            PresentationBus.Publish(new StopPlayingRequest());
		}
	}
}