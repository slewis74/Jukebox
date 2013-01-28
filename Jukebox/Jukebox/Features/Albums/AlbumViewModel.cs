using System.Linq;
using Jukebox.Model;
using Jukebox.Requests;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;
using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Features.Albums
{
    public class AlbumViewModel : ViewModelWithOrientation
	{
        private readonly Album _album;
        
        public AlbumViewModel(
            IPresentationBus presentationBus, 
            INavigator navigator,
            Album album)
            : base(navigator)
		{
			_album = album;

            AlbumLocationCommandMappings = new AsyncObservableCollection<LocationCommandMapping>();
            TrackLocationCommandMappings = new AsyncObservableCollection<LocationCommandMapping>();

			PlaySong = new PlaySongCommand(presentationBus);
			PlayAlbum = new PlayAlbumCommand(presentationBus);
            AddSong = new AddSongCommand(presentationBus);
            AddAlbum = new AddAlbumCommand(presentationBus);

            Tracks = new AsyncObservableCollection<TrackViewModel>(
                album.Songs
                .OrderBy(s => s.DiscNumber)
                .ThenBy(s => s.TrackNumber)
                .Select(t => new TrackViewModel(t, TrackLocationCommandMappings)));
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

    public class PlaySongCommand : Command<TrackViewModel>
	{
        private readonly IPresentationBus _presentationBus;

        public PlaySongCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(TrackViewModel parameter)
		{
            _presentationBus.Publish(new PlaySongNowRequest { Scope = parameter.GetSong() });
		}
	}

    public class PlayAlbumCommand : Command<AlbumViewModel>
	{
        private readonly IPresentationBus _presentationBus;

        public PlayAlbumCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(AlbumViewModel parameter)
		{
            _presentationBus.Publish(new PlayAlbumNowRequest { Scope = parameter.GetAlbum() });
		}

	}

    public class AddSongCommand : Command<TrackViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public AddSongCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(TrackViewModel parameter)
        {
            _presentationBus.Publish(new AddSongToCurrentPlaylistRequest { Song = parameter.GetSong() });
        }
    }

    public class AddAlbumCommand : Command<AlbumViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public AddAlbumCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(AlbumViewModel parameter)
        {
            _presentationBus.Publish(new AddAlbumToCurrentPlaylistRequest { Album = parameter.GetAlbum() });
        }
    }
}