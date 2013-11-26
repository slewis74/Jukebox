using System;
using SlabRt.ViewModels;

namespace Jukebox.Features.Albums
{
    public class PinAlbumCommand : TogglePinCommand
    {
        private readonly AlbumViewModel _albumViewModel;

        public PinAlbumCommand(
            AlbumViewModel albumViewModel)
        {
            _albumViewModel = albumViewModel;
        }

        public override string AppbarTileId
        {
            get
            {
                return string.Format("Album.{0}.{1}", 
                    _albumViewModel.ArtistName.Replace(' ', '.'),
                    _albumViewModel.Title.Replace(' ', '.').Replace(':', '.'));
            }
        }

        public override string TileTitle
        {
            get { return _albumViewModel.Title; }
        }

        public override string ActivationArguments
        {
            get { return "Album/ShowAlbum?artistName=" + _albumViewModel.ArtistName + "&albumTitle=" + _albumViewModel.Title; }
        }

        public override Uri TileImageUri
        {
            get { return new Uri("ms-appx:///Assets/Logo.png"); }
        }
    }
}