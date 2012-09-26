using System.Linq;
using System.Windows.Input;
using Jukebox.Common;
using Jukebox.Events;
using Jukebox.Features.Artists;
using Jukebox.Features.MainPage.Events;
using Jukebox.Features.MainPage.Requests;
using Jukebox.Features.Playlists;
using Jukebox.Model;
using Jukebox.Requests;
using Jukebox.Storage;
using Slew.WinRT.Container;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;
using Windows.UI.Xaml.Media;

namespace Jukebox.Features.MainPage
{
	public class MainPageViewModel :
        CanHandleNavigationBase,
        IHandlePresentationRequest<PlayRequest>,
        IHandlePresentationRequest<PauseRequest>,
        IHandlePresentationRequest<StopRequest>,
        IHandlePresentationRequest<PlaySongNowRequest>,
        IHandlePresentationEvent<SongEndedEvent>,
        IHandlePresentationEvent<CurrentTrackChangedEvent>
	{
        private readonly DistinctAsyncObservableCollection<Playlist> _playlists;
        private readonly PlaylistHandler _playlistHandler;

        public MainPageViewModel(
            DistinctAsyncObservableCollection<Artist> artists,
            DistinctAsyncObservableCollection<Playlist> playlists, 
            Playlist currentPlaylist, 
            PlaylistHandler playlistHandler)
		{
            CurrentPlaylist = currentPlaylist;
            _artists = artists;
            _playlists = playlists;
            _playlistHandler = playlistHandler;
            
            Navigator = new Navigator(new JukeboxControllerFactory(), this);

			DisplayArtists = new DisplayArtistsCommand(Navigator, _artists);

            PlayCommand = PropertyInjector.Inject(() => new PresentationRequestCommand<PlayRequest>());
            PauseCommand = PropertyInjector.Inject(() => new PresentationRequestCommand<PauseRequest>());
            PlaylistsCommand = PropertyInjector.Inject(() => new PlaylistsCommand(Navigator, _playlists));
            NextTrackCommand = PropertyInjector.Inject(() => new NextTrackCommand());
            PreviousTrackCommand = PropertyInjector.Inject(() => new PreviousTrackCommand());
        }

        public INavigator Navigator { get; set; }

		public DisplayArtistsCommand DisplayArtists { get; private set; }

        public ICommand PlayCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }
        public PlaylistsCommand PlaylistsCommand { get; private set; }
        public NextTrackCommand NextTrackCommand { get; private set; }
        public PreviousTrackCommand PreviousTrackCommand { get; private set; }

	    private Playlist _currentPlaylist;
	    public Playlist CurrentPlaylist
	    {
	        get { return _currentPlaylist; }
	        set
	        {
                if (_currentPlaylist == value)
                    return;
                
                _currentPlaylist = value;
                NotifyChanged(() => CurrentPlaylist);
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
	            _isPaused = value;
                NotifyChanged(() => IsPaused);
	        }
	    }

	    private bool _isPlaying;
	    private ImageSource _currentTrackAlbumImageSource;
	    private string _currentTrackDescription;

	    public bool IsPlaying
	    {
	        get { return _isPlaying; }
	        set
	        {
	            _isPlaying = value;
                NotifyChanged(() => IsPlaying);
                NotifyChanged(() => IsNotPlaying);
	        }
	    }

	    public bool IsNotPlaying
	    {
	        get { return !IsPlaying; }
	        set { IsPlaying = !value; }
	    }

        public ImageSource CurrentTrackAlbumImageSource
        {
            get { return _currentTrackAlbumImageSource; }
            set
            {
                _currentTrackAlbumImageSource = value;
                NotifyChanged(() => CurrentTrackAlbumImageSource);
            }
        }

        public string CurrentTrackDescription
        {
            get { return _currentTrackDescription; }
            set
            {
                _currentTrackDescription = value;
                NotifyChanged(() => CurrentTrackDescription);
            }
        }

	    public void Handle(SongEndedEvent e)
		{
            if (CurrentPlaylist != null && CurrentPlaylist.CanMoveNext(false))
			{
                CurrentPlaylist.MoveToNextTrack(false);
                PlayFile(CurrentPlaylist.CurrentTrack);
			}
		}

        public void Handle(CurrentTrackChangedEvent e)
        {
            var song = e.Data;

            if (song == null)
                StopPlaying();
            else
            {
                PlayFile(song);
            }
            _playlistHandler.SaveData(_playlists, _currentPlaylist);
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

        public void Handle(PlaySongNowRequest request)
        {
            request.IsHandled = true;
            StopAndResetDefaultPlaylist();
            AddToCurrentPlaylist(request.Scope);
            StartPlaying();
        }

        public void Handle(PlayAlbumNowRequest request)
        {
            request.IsHandled = true;
            StopAndResetDefaultPlaylist();
            AddToCurrentPlaylist(request.Scope);
            StartPlaying();
        }

		private void StopAndResetDefaultPlaylist()
		{
			StopPlaying();
		    CurrentPlaylist = _playlists.Single(p => p.Name == "Default");
            CurrentPlaylist.Clear();
            _playlistHandler.SaveData(_playlists, CurrentPlaylist);
        }

		public void AddToCurrentPlaylist(Song song)
		{
            CurrentPlaylist.Add(song);
            _playlistHandler.SaveData(_playlists, CurrentPlaylist);
		}

		public void AddToCurrentPlaylist(Album album)
		{
			foreach (var song in album.Songs.OrderBy(s => s.TrackNumber))
			{
				AddToCurrentPlaylist(song);
			}
		}

		public void AddToCurrentPlaylist(Artist artist)
		{
			foreach (var song in artist.Albums.SelectMany(a => a.Songs).OrderBy(s => s.Album.Title).ThenBy(s => s.TrackNumber))
			{
				AddToCurrentPlaylist(song);
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
                if (CurrentPlaylist.CurrentTrack == null)
                    NextTrack();
                else
                {
                    PlayFile(CurrentPlaylist.CurrentTrack);
                }
            }
        }
        
		private async void PlayFile(Song song)
		{
            IsPaused = false;
            IsPlaying = true;
		    CurrentTrackAlbumImageSource = song.Album.SmallBitmap;
		    CurrentTrackDescription = string.Format("{0} - {1}", song.Album.Artist.Name, song.Title);
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

        public void PreviousTrack()
        {
        }

	    public void NextTrack()
	    {
            if (CurrentPlaylist == null && _playlists.Any())
            {
                CurrentPlaylist = _playlists.Single(p => p.Name == "Default");
            }
            if (CurrentPlaylist != null)
            {
                CurrentPlaylist.MoveToNextTrack(false);
            }
	    }
	}

	public class DisplayArtistsCommand : Command
	{
	    private readonly INavigator _navigator;
        private readonly DistinctAsyncObservableCollection<Artist> _artists;

        public DisplayArtistsCommand(INavigator navigator, DistinctAsyncObservableCollection<Artist> artists)
		{
		    _navigator = navigator;
			_artists = artists;
		}

		public override void Execute(object parameter)
		{
            _navigator.Navigate<ArtistController>(c => c.ShowAll(_artists));
		}
	}

    public class PlaylistsCommand : Command
    {
        private readonly INavigator _navigator;
        private readonly DistinctAsyncObservableCollection<Playlist> _playlists;

        public PlaylistsCommand(INavigator navigator, DistinctAsyncObservableCollection<Playlist> playlists)
        {
            _navigator = navigator;
            _playlists = playlists;
        }

        public override void Execute(object parameter)
        {
            _navigator.Navigate<PlaylistController>(c => c.ShowAll(_playlists));
        }
    }

    public class NextTrackCommand : PresentationRequestCommand<NextTrackRequest>
    {
        public override bool CanExecute(object parameter)
        {
            return false;
        }
    }
    public class PreviousTrackCommand : PresentationRequestCommand<PreviousTrackRequest>
    {
        public override bool CanExecute(object parameter)
        {
            return false;
        }
    }

}