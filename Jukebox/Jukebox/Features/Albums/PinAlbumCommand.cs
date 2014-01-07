using System;
using Jukebox.Storage;
using SlabRt.ViewModels;

namespace Jukebox.Features.Albums
{
    public class PinAlbumCommand : TogglePinCommand
    {
        private readonly IAlbumArtStorage _albumArtStorage;
        private readonly AlbumViewModel _albumViewModel;

        public PinAlbumCommand(
            IAlbumArtStorage albumArtStorage,
            AlbumViewModel albumViewModel)
        {
            _albumArtStorage = albumArtStorage;
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
            get { return new Uri("ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(_albumViewModel.ArtistName, _albumViewModel.Title, 150)); }
        }
    }
}