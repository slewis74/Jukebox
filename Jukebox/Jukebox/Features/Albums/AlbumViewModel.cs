using System.Linq;
using Jukebox.Model;
using Jukebox.Requests;
using Slew.WinRT.Container;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;
using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Features.Albums
{
    public class AlbumViewModel : CanRequestNavigationBase
	{
        private readonly Album _album;
        
        public AlbumViewModel(Album album)
		{
			_album = album;

            AlbumLocationCommandMappings = new AsyncObservableCollection<LocationCommandMapping>();
            TrackLocationCommandMappings = new AsyncObservableCollection<LocationCommandMapping>();

			PlaySong = PropertyInjector.Inject(()=> new PlaySongCommand());
			PlayAlbum = PropertyInjector.Inject(()=> new PlayAlbumCommand());
            AddSong = PropertyInjector.Inject(()=> new AddSongCommand());
            AddAlbum = PropertyInjector.Inject(()=> new AddAlbumCommand());

            Tracks = new AsyncObservableCollection<TrackViewModel>(
                album.Songs
                .OrderBy(s => s.DiscNumber)
                .ThenBy(s => s.TrackNumber)
                .Select(t => 
                    PropertyInjector.Inject(()=> new TrackViewModel(t, TrackLocationCommandMappings))));
		}

		public PlaySongCommand PlaySong { get; private set; }
		public PlayAlbumCommand PlayAlbum { get; private set; }
		public AddSongCommand AddSong { get; private set; }
        public AddAlbumCommand AddAlbum { get; private set; }

        public string Title { get { return _album.Title; } }
        public string ArtistName { get { return _album.Artist.Name; } }

        public BitmapImage SmallBitmap { get { return _album.SmallBitmap; } }
        public BitmapImage LargeBitmap { get { return _album.LargeBitmap; } }

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

        public Album GetAlbum()
        {
            return _album;
        }

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
            PresentationBus.Publish(new PlaySongNowRequest { Scope = parameter.GetSong() });
		}
	}

    public class PlayAlbumCommand : Command<AlbumViewModel>, IPublish
	{
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(AlbumViewModel parameter)
		{
            PresentationBus.Publish(new PlayAlbumNowRequest { Scope = parameter.GetAlbum() });
		}

	}

    public class AddSongCommand : Command<TrackViewModel>, IPublish
    {
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(TrackViewModel parameter)
        {
            PresentationBus.Publish(new AddSongToCurrentPlaylistRequest { Song = parameter.GetSong() });
        }
    }

    public class AddAlbumCommand : Command<AlbumViewModel>, IPublish
    {
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(AlbumViewModel parameter)
        {
            PresentationBus.Publish(new AddAlbumToCurrentPlaylistRequest { Album = parameter.GetAlbum() });
        }
    }
}