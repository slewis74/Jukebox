using System.Diagnostics;
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

namespace Jukebox.Features.MainPage
{
	public class MainPageViewModel :
        CanHandleNavigationBase,
        IHandlePresentationRequest<PlayRequest>,
        IHandlePresentationRequest<PauseRequest>,
        IHandlePresentationRequest<StopRequest>,
        IHandlePresentationRequest<PlaySongNowRequest>,
        IHandlePresentationRequest<PlayAlbumNowRequest>,
        IHandlePresentationEvent<SongEndedEvent>,
        IHandlePresentationEvent<PlaylistCurrentTrackChangedEvent>,
        IHandlePresentationRequest<PreviousTrackRequest>,
        IHandlePresentationRequest<NextTrackRequest>
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
            NextTrackCommand = PropertyInjector.Inject(() => new NextTrackCommand(CurrentPlaylist.CanMoveNext));
            PreviousTrackCommand = PropertyInjector.Inject(() => new PreviousTrackCommand(CurrentPlaylist.CanMovePrevious));
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

        private string _currentTrackDescription;
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
            if (CurrentPlaylist != null && CurrentPlaylist.CanMoveNext)
			{
                CurrentPlaylist.MoveToNextTrack();
                PlayFile(CurrentPlaylist.CurrentTrack);
			}
		}

        public void Handle(PlaylistCurrentTrackChangedEvent e)
        {
            if (e.Data != _currentPlaylist)
                return;

            var song = e.Song;

            if (song == null)
                StopPlaying();
            else
            {
                PlayFile(song);
            }
            Debug.WriteLine("Track changed, saving playlist data");
            _playlistHandler.SaveCurrentTrackForPlaylist(_currentPlaylist);
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

        public void Handle(PreviousTrackRequest request)
        {
            PreviousTrack();
        }
        public void Handle(NextTrackRequest request)
        {
            NextTrack();
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
        }

		public void AddToCurrentPlaylist(Song song)
		{
            CurrentPlaylist.Add(song);
            Debug.WriteLine("Song added to playlist, saving data");
            _playlistHandler.SaveData(_playlists, CurrentPlaylist);
		}

		public void AddToCurrentPlaylist(Album album)
		{
            foreach (var song in album.Songs.OrderBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber))
			{
                CurrentPlaylist.Add(song);
            }
            Debug.WriteLine("Album added to playlist, saving data");
            _playlistHandler.SaveData(_playlists, CurrentPlaylist);
        }

		public void AddToCurrentPlaylist(Artist artist)
		{
			foreach (var song in artist.Albums.SelectMany(a => a.Songs).OrderBy(s => s.Album.Title).ThenBy(s => s.TrackNumber))
			{
                CurrentPlaylist.Add(song);
            }
            Debug.WriteLine("Artist added to playlist, saving data");
            _playlistHandler.SaveData(_playlists, CurrentPlaylist);
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
            if (CurrentPlaylist != null)
            {
                CurrentPlaylist.MoveToPreviousTrack();
            }
        }

	    public void NextTrack()
	    {
            if (CurrentPlaylist == null && _playlists.Any())
            {
                CurrentPlaylist = _playlists.Single(p => p.Name == "Default");
            }
            if (CurrentPlaylist != null)
            {
                CurrentPlaylist.MoveToNextTrack();
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

    public class NextTrackCommand : 
        PresentationRequestCommand<NextTrackRequest>,
        IHandlePresentationEvent<PlaylistCurrentTrackChangedEvent>,
        IHandlePresentationEvent<PlaylistContentChangedEvent>
    {
        private bool _canMoveNext;

        public NextTrackCommand(bool canMoveNext)
        {
            _canMoveNext = canMoveNext;
        }

        public override bool CanExecute(object parameter)
        {
            return _canMoveNext;
        }

        public void Handle(PlaylistCurrentTrackChangedEvent e)
        {
            _canMoveNext = e.CanMoveNext;
            RaiseCanExecuteChanged();
        }
        public void Handle(PlaylistContentChangedEvent e)
        {
            _canMoveNext = e.CanMoveNext;
            RaiseCanExecuteChanged();
        }
    }

    public class PreviousTrackCommand : 
        PresentationRequestCommand<PreviousTrackRequest>,
        IHandlePresentationEvent<PlaylistCurrentTrackChangedEvent>,
        IHandlePresentationEvent<PlaylistContentChangedEvent>
    {
        private bool _canMovePrevious;

        public PreviousTrackCommand(bool canMovePrevious)
        {
            _canMovePrevious = canMovePrevious;
        }

        public override bool CanExecute(object parameter)
        {
            return _canMovePrevious;
        }

        public void Handle(PlaylistCurrentTrackChangedEvent e)
        {
            _canMovePrevious = e.CanMovePrevious;
            RaiseCanExecuteChanged();
        }
        public void Handle(PlaylistContentChangedEvent e)
        {
            _canMovePrevious = e.CanMovePrevious;
            RaiseCanExecuteChanged();
        }
    }
}