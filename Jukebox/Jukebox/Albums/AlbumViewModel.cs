using System.Linq;
using Jukebox.Model;
using Jukebox.Requests;
using Slew.WinRT.Container;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;

namespace Jukebox.Albums
{
    public class AlbumViewModel : CanRequestNavigationBase
	{
		public AlbumViewModel(Album album)
		{
			Album = album;

            AlbumLocationCommandMappings = new AsyncObservableCollection<LocationCommandMapping>();
            TrackLocationCommandMappings = new AsyncObservableCollection<LocationCommandMapping>();

			PlaySong = PropertyInjector.Resolve(()=> new PlaySongCommand());
			PlayAlbum = PropertyInjector.Resolve(()=> new PlayAlbumCommand());
            AddSong = PropertyInjector.Resolve(()=> new AddSongCommand());
            AddAlbum = PropertyInjector.Resolve(()=> new AddAlbumCommand());

            Tracks = new AsyncObservableCollection<TrackViewModel>(
                Album.Songs
                .OrderBy(s => s.DiscNumber)
                .ThenBy(s => s.TrackNumber)
                .Select(t => 
                    PropertyInjector.Resolve(()=> new TrackViewModel(t, TrackLocationCommandMappings))));
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

    public class PlaySongCommand : Command<TrackViewModel>, IPublish
	{
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(TrackViewModel parameter)
		{
            PresentationBus.Publish(new PlaySongNowRequest { Scope = parameter.Song });
		}
	}

    public class PlayAlbumCommand : Command<AlbumViewModel>, IPublish
	{
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(AlbumViewModel parameter)
		{
            PresentationBus.Publish(new PlayAlbumNowRequest { Scope = parameter.Album });
		}

	}

    public class AddSongCommand : Command<TrackViewModel>, IPublish
    {
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(TrackViewModel parameter)
        {
            PresentationBus.Publish(new AddSongToCurrentPlaylistRequest { Song = parameter.Song });
        }
    }

    public class AddAlbumCommand : Command<AlbumViewModel>, IPublish
    {
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(AlbumViewModel parameter)
        {
            PresentationBus.Publish(new AddAlbumToCurrentPlaylistRequest { Album = parameter.Album });
        }
    }
}