using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Requests;
using Jukebox.WinStore.Storage;
using Slab.Data;
using Slab.Pages.Navigation;
using Slab.PresentationBus;
using Slab.WinStore.Pages;
using Slab.Xaml.ViewModels;

namespace Jukebox.WinStore.Features.Albums
{
    public class AlbumViewModel : CanRequestNavigationBase, IShare
	{
        private readonly Album _album;

        public delegate AlbumViewModel Factory(Album album);
        
        public AlbumViewModel(
            IPresentationBus presentationBus, 
            INavigator navigator,
            IAlbumArtStorage albumArtStorage,
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

            PinAlbum = new PinAlbumCommand(albumArtStorage, this);

            Tracks = new AsyncObservableCollection<TrackViewModel>(
                album.Songs
                .OrderBy(s => s.DiscNumber)
                .ThenBy(s => s.TrackNumber)
                .Select(t => new TrackViewModel(t, TrackLocationCommandMappings)));
		}

        public override string PageTitle
        {
            get { return "Album"; }
        }

		public PlaySongCommand PlaySong { get; private set; }
		public PlayAlbumCommand PlayAlbum { get; private set; }
		public AddSongCommand AddSong { get; private set; }
        public AddAlbumCommand AddAlbum { get; private set; }

        public PinAlbumCommand PinAlbum { get; private set; }

        public string Title { get { return _album.Title; } }
        public string ArtistName { get { return _album.Artist.Name; } }

        public string SmallBitmapUri { get { return _album.SmallBitmapUri; } }
        public string LargeBitmapUri { get { return _album.LargeBitmapUri; } }

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

        public bool GetShareContent(DataRequest dataRequest)
        {
            dataRequest.Data.Properties.Title = _album.Artist.Name;
            dataRequest.Data.SetText(_album.Title + "\n" + _album.Artist.Name);
            return true;
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
            _presentationBus.PublishAsync(new PlaySongNowRequest { Scope = parameter.GetSong() });
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
            _presentationBus.PublishAsync(new PlayAlbumNowRequest { Scope = parameter.GetAlbum() });
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
            _presentationBus.PublishAsync(new AddSongToCurrentPlaylistRequest { Song = parameter.GetSong() });
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
            _presentationBus.PublishAsync(new AddAlbumToCurrentPlaylistRequest { Album = parameter.GetAlbum() });
        }
    }
}