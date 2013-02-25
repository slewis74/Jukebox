using System.Collections.ObjectModel;
using Jukebox.Features.Albums;
using Jukebox.Model;
using Jukebox.Requests;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;
using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Features.Artists.Single
{
    public class ArtistViewModel : CanRequestNavigationBase
	{
		private readonly Artist _artist;

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

        public BitmapImage SmallBitmap { get { return _artist.SmallBitmap; } }
        public BitmapImage LargeBitmap { get { return _artist.LargeBitmap; } }

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
            _presentationBus.Publish(new PlayArtistNowRequest { Scope = parameter.GetArtist() });
		}
    }
}