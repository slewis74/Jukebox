using System;
using System.Collections.ObjectModel;
using Jukebox.Albums;
using Jukebox.Common;
using Jukebox.Model;
using Slew.WinRT.Pages;
using Slew.WinRT.ViewModels;

namespace Jukebox.Artists
{
	public class ArtistViewModel : CanContributeToPlaylistBase
	{
		private readonly Artist _artist;

		public ArtistViewModel(Artist artist, IHandlePlaylists handlesPlaylists) : base(handlesPlaylists)
		{
			_artist = artist;
			DisplayAlbum = new DisplayAlbumCommand(new Lazy<INavigator>(() => Navigator));
		}

		public DisplayAlbumCommand DisplayAlbum { get; private set; }

		public string Name { get { return _artist.Name; } }

		public ObservableCollection<Album> Albums { get { return _artist.Albums; } }
	}

	public class DisplayAlbumCommand : Command<Album>
	{
	    private readonly Lazy<INavigator> _navigator;
	    
        public DisplayAlbumCommand(Lazy<INavigator> navigator)
		{
		    _navigator = navigator;
		}

	    public override void Execute(Album album)
		{
			_navigator.Value.Navigate<AlbumController>(c => c.ShowAlbum(album));
		}
	}
}