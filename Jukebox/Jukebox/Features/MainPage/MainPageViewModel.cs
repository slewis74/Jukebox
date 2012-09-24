using System;
using System.Linq;
using Jukebox.Common;
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
        IHandlePresentationEvent<SongEndedEvent>
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

            PlayCommand = PropertyInjector.Resolve(() => new PlayCommand());
            PauseCommand = PropertyInjector.Resolve(() => new PauseCommand());
            PlaylistsCommand = PropertyInjector.Resolve(() => new PlaylistsCommand(Navigator, _playlists));
		}

        public INavigator Navigator { get; set; }

		public DisplayArtistsCommand DisplayArtists { get; private set; }

        public PlayCommand PlayCommand { get; private set; }
        public PauseCommand PauseCommand { get; private set; }
        public PlaylistsCommand PlaylistsCommand { get; private set; }

	    private Playlist _currentPlaylist;
	    public Playlist CurrentPlaylist
	    {
	        get { return _currentPlaylist; }
	        set
	        {
                if (_currentPlaylist == value)
                    return;

                if (_currentPlaylist != null)
                {
                    _currentPlaylist.CurrentTrackChanged -= CurrentPlaylistOnCurrentTrackChanged;
                }
                
                _currentPlaylist = value;
                NotifyChanged(() => CurrentPlaylist);
                
                if (_currentPlaylist != null)
                {
                    _currentPlaylist.CurrentTrackChanged += CurrentPlaylistOnCurrentTrackChanged;
                }
	        }
	    }

	    private void CurrentPlaylistOnCurrentTrackChanged(object sender, Song song)
	    {
            if (song == null)
                OnStopPlaying();
            else
            {
                OnPlayFile(song);
            }
            _playlistHandler.SaveData(_playlists, _currentPlaylist);
        }

	    public Uri FileSource { get; set; }

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

	    public void Handle(SongEndedEvent e)
		{
            if (CurrentPlaylist != null && CurrentPlaylist.CanMoveNext(false))
			{
                CurrentPlaylist.MoveToNextTrack(false);
                OnPlayFile(CurrentPlaylist.CurrentTrack);
			}
		}

		private void StopAndResetCurrentPlaylist()
		{
			OnStopPlaying();
            CurrentPlaylist = null;
		}

		public void AddToCurrentPlaylist(Song song)
		{
            CurrentPlaylist.Add(song);
            _playlistHandler.SaveData(_playlists, _currentPlaylist);
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

        public void Handle(PlayRequest request)
        {
            if (IsPlaying)
                return;
            if (IsPaused)
                OnRestartPlaying();
            else
            {
                if (_currentPlaylist.CurrentTrack == null)
                    NextTrack();
                else
                {
                    OnPlayFile(_currentPlaylist.CurrentTrack);
                }
            }
        }
        public void Handle(PauseRequest request)
        {
            if (IsNotPlaying)
                return;
            OnPausePlaying();
        }
        public void Handle(StopRequest request)
        {
            if (IsNotPlaying)
                return;
            OnStopPlaying();
        }

        public void Handle(PlaySongNowRequest request)
        {
            request.IsHandled = true;
            StopAndResetCurrentPlaylist();
            OnPlayFile(request.Scope);
        }

		private async void OnPlayFile(Song song)
		{
            IsPaused = false;
            IsPlaying = true;
		    PresentationBus.Publish(new PlayFileRequest(await song.GetStorageFileAsync()));
		}

        private void OnRestartPlaying()
        {
            IsPlaying = true;
            IsPaused = false;
            PresentationBus.Publish(new RestartPlayingRequest());
        }

        private void OnPausePlaying()
        {
            IsPlaying = false;
            IsPaused = true;
            PresentationBus.Publish(new PausePlayingRequest());
        }

		private void OnStopPlaying()
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

    public class PlayCommand : Command, IPublish
    {
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(object parameter)
        {
            PresentationBus.Publish(new PlayRequest());
        }
    }

    public class PauseCommand : Command, IPublish
    {
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(object parameter)
        {
            PresentationBus.Publish(new PauseRequest());
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
}