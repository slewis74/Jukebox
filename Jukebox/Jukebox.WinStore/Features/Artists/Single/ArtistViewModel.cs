using System.Collections.ObjectModel;
using Jukebox.WinStore.Features.Albums;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Requests;
using Slab.Pages.Navigation;
using Slab.PresentationBus;
using Slab.Xaml.ViewModels;

namespace Jukebox.WinStore.Features.Artists.Single
{
    public class ArtistViewModel : CanRequestNavigationBase
	{
		private readonly Artist _artist;

        public delegate ArtistViewModel Factory(Artist artist);

		public ArtistViewModel(
            IPresentationBus presentationBus, 
            INavigator navigator, 
            Artist artist) : base(navigator)
		{
			_artist = artist;
			DisplayAlbum = new DisplayAlbumCommand(Navigator);

            PlayArtist = new PlayArtistCommand(presentationBus);
		}

        public override string PageTitle
        {
            get { return "Artist"; }
        }

		public DisplayAlbumCommand DisplayAlbum { get; private set; }

		public string Name { get { return _artist.Name; } }

		public ObservableCollection<Album> Albums { get { return _artist.Albums; } }

        public string SmallBitmapUri { get { return _artist.SmallBitmapUri; } }
        public string LargeBitmapUri { get { return _artist.LargeBitmapUri; } }

        public PlayArtistCommand PlayArtist { get; private set; }

        public Artist GetArtist()
        {
            return _artist;
        }
	}

    public class DisplayAlbumCommand : NavigationCommand<Album>
	{
        public DisplayAlbumCommand(INavigator navigator) : base(navigator)
		{}

	    public override void Execute(Album album)
		{
			Navigator.Navigate<AlbumController>(c => c.ShowAlbum(album.Artist.Name, album.Title));
		}
	}

    public class PlayArtistCommand : Command<ArtistViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public PlayArtistCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(ArtistViewModel parameter)
		{
            _presentationBus.PublishAsync(new PlayArtistNowRequest { Scope = parameter.GetArtist() });
		}
    }
}