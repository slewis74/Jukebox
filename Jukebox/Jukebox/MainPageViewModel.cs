using System;
using System.Linq;
using Jukebox.Artists;
using Jukebox.Common;
using Jukebox.Model;
using Jukebox.Playlists;
using Jukebox.Storage;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.ViewModels;
using Windows.Storage;

namespace Jukebox
{
	public class MainPageViewModel : CanHandleNavigationBase, IHandlePlaylists
	{
	    public event EventHandler<StorageFile> PlayFile;
		public event EventHandler RestartPlaying;
		public event EventHandler PausePlaying;
		public event EventHandler StopPlaying;

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
            
            Navigator = new Navigator(new JukeboxControllerFactory(this), this);
			DisplayArtists = new DisplayArtistsCommand(Navigator, _artists);

            PlayCommand = new PlayCommand(this);
            PauseCommand = new PauseCommand(this);
            PlaylistsCommand = new PlaylistsCommand(Navigator, _playlists);
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

	    public void SongFinished()
		{
            if (CurrentPlaylist != null && CurrentPlaylist.CanMoveNext(false))
			{
                CurrentPlaylist.MoveToNextTrack(false);
                OnPlayFile(CurrentPlaylist.CurrentTrack);
			}
		}

		private void StopAndClearCurrentPlaylist()
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

        public void Play()
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
        public void Pause()
        {
            if (IsNotPlaying)
                return;
            OnPausePlaying();
        }
        public void Stop()
        {
            if (IsNotPlaying)
                return;
            OnStopPlaying();
        }

		public void PlayNow(Song song)
		{
			StopAndClearCurrentPlaylist();
            OnPlayFile(song);
		}

		public void PlayNow(Album album) { }
		public void PlayNow(Artist artist) { }

		private async void OnPlayFile(Song song)
		{
            IsPaused = false;
            IsPlaying = true;
            if (PlayFile != null)
			{
				PlayFile(this, await song.GetStorageFileAsync());
			}
		}

        private void OnRestartPlaying()
        {
            IsPlaying = true;
            IsPaused = false;
            if (RestartPlaying != null)
            {
                RestartPlaying(this, EventArgs.Empty);
            }
        }

        private void OnPausePlaying()
        {
            IsPlaying = false;
            IsPaused = true;
            if (PausePlaying != null)
            {
                PausePlaying(this, EventArgs.Empty);
            }
        }

		private void OnStopPlaying()
		{
		    IsPlaying = false;
		    IsPaused = false;
			if (StopPlaying != null)
			{
				StopPlaying(this, EventArgs.Empty);
			}
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

	public interface IHandlePlaylists
	{
	    void Play();
	    void Pause();
	    void Stop();

		void PlayNow(Song song);
		void PlayNow(Album album);
		void PlayNow(Artist artist);

		void AddToCurrentPlaylist(Song song);
		void AddToCurrentPlaylist(Album album);
		void AddToCurrentPlaylist(Artist artist);
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

    public class PlayCommand : Command
    {
        private readonly IHandlePlaylists _handlePlaylists;

        public PlayCommand(IHandlePlaylists handlePlaylists)
        {
            _handlePlaylists = handlePlaylists;
        }

        public override void Execute(object parameter)
        {
            _handlePlaylists.Play();
        }
    }

    public class PauseCommand : Command
    {
        private readonly IHandlePlaylists _handlePlaylists;

        public PauseCommand(IHandlePlaylists handlePlaylists)
        {
            _handlePlaylists = handlePlaylists;
        }

        public override void Execute(object parameter)
        {
            _handlePlaylists.Pause();
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