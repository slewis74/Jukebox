using Slew.WinRT.ViewModels;

namespace Jukebox.Common
{
	public abstract class CanContributeToPlaylistBase : CanRequestNavigationBase
	{
		protected CanContributeToPlaylistBase(IHandlePlaylists handlesPlaylists)
		{
			HandlesPlaylists = handlesPlaylists;
		}

		public IHandlePlaylists HandlesPlaylists { get; set; }

		public AddToCurrentPlaylistCommand AddToCurrentPlaylistCommand { get; set; }
	}

	public class AddToCurrentPlaylistCommand : Command
	{
		private readonly CanContributeToPlaylistBase _canContributeToPlaylist;

		public AddToCurrentPlaylistCommand(CanContributeToPlaylistBase canContributeToPlaylist)
		{
			_canContributeToPlaylist = canContributeToPlaylist;
		}

		public override void Execute(object parameter)
		{
			//_canContributeToPlaylist.HandlesPlaylists.AddToCurrentPlaylist();
		}
	}
}