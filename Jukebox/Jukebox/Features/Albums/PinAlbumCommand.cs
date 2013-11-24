using System;
using Windows.Foundation;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Slab.ViewModels;

namespace Jukebox.Features.Albums
{
    public class PinAlbumCommand : Command
    {
        private readonly AlbumViewModel _albumViewModel;

        public PinAlbumCommand(
            AlbumViewModel albumViewModel)
        {
            _albumViewModel = albumViewModel;
        }

        public string AppbarTileId { get { return string.Format("SecondaryTile.AppBar.JukeboxAlbum.{0}.{1}", _albumViewModel.ArtistName.Replace(' ', '.'), _albumViewModel.Title.Replace(' ', '.')); } }

        public async override void Execute(object parameter)
        {
            var button = (Button)parameter;

            var parent = (FrameworkElement)button.Parent;
            while ((parent is AppBar) == false)
            {
                parent = (FrameworkElement)parent.Parent;
            }
            var appBar = (AppBar)parent;

            appBar.IsSticky = true;

            if (SecondaryTile.Exists(AppbarTileId))
            {
                var secondaryTile = new SecondaryTile(AppbarTileId);
                var isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(button.GetElementRect(), Windows.UI.Popups.Placement.Above);

                //ToggleAppBarButton(isUnpinned);
            }
            else
            {
                var logo = new Uri("ms-appx:///Assets/Logo.png");
                var wideLogo = new Uri("ms-appx:///Assets/WideLogo.png");
                var tileActivationArguments = "Album/ShowAlbum?artistName=" + _albumViewModel.ArtistName + "&albumTitle=" + _albumViewModel.Title;

                var secondaryTile = new SecondaryTile(AppbarTileId,
                                                        _albumViewModel.ArtistName,
                                                        _albumViewModel.Title,
                                                        tileActivationArguments,
                                                        TileOptions.ShowNameOnWideLogo,
                                                        logo,
                                                        wideLogo)
                {
                    ForegroundText = ForegroundText.Dark,
                    SmallLogo = new Uri("ms-appx:///Assets/SmallLogo.png")
                };

                bool isPinned = await secondaryTile.RequestCreateForSelectionAsync((button).GetElementRect(), Windows.UI.Popups.Placement.Above);

                //ToggleAppBarButton(!isPinned);
            }
            appBar.IsSticky = false;
        }
    }

    public static class Positioning
    {
        public static Rect GetElementRect(this FrameworkElement element, int hOffset, int vOffset)
        {
            var rect = GetElementRect(element);
            rect.Y += vOffset;
            rect.X += hOffset;
            return rect;
        }

        public static Rect GetElementRect(this FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
    }

}