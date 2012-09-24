using System;
using System.Collections.ObjectModel;
using Jukebox.Albums;
using Jukebox.Model;
using Slew.WinRT.Pages;
using Slew.WinRT.ViewModels;

namespace Jukebox.Artists
{
	public class ArtistViewModel : CanRequestNavigationBase
	{
		private readonly Artist _artist;

		public ArtistViewModel(Artist artist)
		{
			_artist = artist;
			DisplayAlbum = new DisplayAlbumCommand(new Lazy<INavigator>(() => Navigator));
		}

		public DisplayAlbumCommand DisplayAlbum { get; private set; }

		public string Name { get { return _artist.Name; } }

		public ObservableCollection<Album> Albums { get { return _artist.Albums; } }
	}

    public class DisplayAlbumCommand : NavigationCommand<Album>
	{
        public DisplayAlbumCommand(Lazy<INavigator> navigator) : base(navigator)
		{}

	    public override void Execute(Album album)
		{
			Navigator.Value.Navigate<AlbumController>(c => c.ShowAlbum(album));
		}
	}
}