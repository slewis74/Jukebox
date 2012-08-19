using System.Linq;
using Jukebox.Common;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.ViewModels;

namespace Jukebox.Albums
{
	public class AlbumViewModel : CanContributeToPlaylistBase
	{
		public AlbumViewModel(Album album, IHandlePlaylists handlesPlaylists)
			: base(handlesPlaylists)
		{
			Album = album;

            AlbumLocationCommandMappings = new AsyncObservableCollection<LocationCommandMapping>();
            TrackLocationCommandMappings = new AsyncObservableCollection<LocationCommandMapping>();

			PlaySong = new PlaySongCommand(handlesPlaylists);
			PlayAlbum = new PlayAlbumCommand(handlesPlaylists);
            AddSong = new AddSongCommand(handlesPlaylists);
            AddAlbum = new AddAlbumCommand(handlesPlaylists);

            Tracks = new AsyncObservableCollection<TrackViewModel>(Album.Songs.OrderBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber).Select(t => new TrackViewModel(t, TrackLocationCommandMappings)));
		}

        public Album Album { get; private set; }

		public PlaySongCommand PlaySong { get; private set; }
		public PlayAlbumCommand PlayAlbum { get; private set; }
		public AddSongCommand AddSong { get; private set; }
        public AddAlbumCommand AddAlbum { get; private set; }

        public AsyncObservableCollection<TrackViewModel> Tracks { get; private set; }

        private TrackViewModel _selectedTrack;
        public TrackViewModel SelectedTrack
		{
			get { return _selectedTrack; }
			set
			{
				_selectedTrack = value;
				NotifyChanged(() => SelectedTrack);
			}
		}

	    public AsyncObservableCollection<LocationCommandMapping> AlbumLocationCommandMappings { get; private set; }
	    private AsyncObservableCollection<LocationCommandMapping> TrackLocationCommandMappings { get; set; }

	    public void SetLocations(Location getPlayDropLocation, Location getPlaylistDropLocation)
	    {
	        AlbumLocationCommandMappings.Replace(new[]
	                                      {
	                                          new LocationCommandMapping { Location = getPlayDropLocation, Command = PlayAlbum },
	                                          new LocationCommandMapping { Location = getPlaylistDropLocation, Command = AddAlbum }
	                                      });
	        TrackLocationCommandMappings.Replace(new[]
	                                      {
	                                          new LocationCommandMapping { Location = getPlayDropLocation, Command = PlaySong },
	                                          new LocationCommandMapping { Location = getPlaylistDropLocation, Command = AddSong }
	                                      });
	    }
	}

    public class PlaySongCommand : Command<TrackViewModel>
	{
		private readonly IHandlePlaylists _handlesPlaylists;

		public PlaySongCommand(IHandlePlaylists handlesPlaylists)
		{
			_handlesPlaylists = handlesPlaylists;
		}

        public override void Execute(TrackViewModel parameter)
		{
			_handlesPlaylists.PlayNow(parameter.Song);
		}
	}

    public class PlayAlbumCommand : Command<AlbumViewModel>
	{
		private readonly IHandlePlaylists _handlesPlaylists;

		public PlayAlbumCommand(IHandlePlaylists handlesPlaylists)
		{
			_handlesPlaylists = handlesPlaylists;
		}

        public override void Execute(AlbumViewModel parameter)
		{
			_handlesPlaylists.PlayNow(parameter.Album);
		}
	}

    public class AddSongCommand : Command<TrackViewModel>
    {
        private readonly IHandlePlaylists _handlesPlaylists;

        public AddSongCommand(IHandlePlaylists handlesPlaylists)
        {
            _handlesPlaylists = handlesPlaylists;
        }

        public override void Execute(TrackViewModel parameter)
        {
            _handlesPlaylists.AddToCurrentPlaylist(parameter.Song);
        }
    }

    public class AddAlbumCommand : Command<AlbumViewModel>
    {
        private readonly IHandlePlaylists _handlesPlaylists;

        public AddAlbumCommand(IHandlePlaylists handlesPlaylists)
        {
            _handlesPlaylists = handlesPlaylists;
        }

        public override void Execute(AlbumViewModel parameter)
        {
            _handlesPlaylists.AddToCurrentPlaylist(parameter.Album);
        }
    }
}