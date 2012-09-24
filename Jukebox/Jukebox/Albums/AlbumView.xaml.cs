using System;
using Jukebox.Requests;
using Slew.WinRT.Pages;
using Slew.WinRT.PresentationBus;
using Windows.UI.Xaml;

namespace Jukebox.Albums
{
	public sealed partial class AlbumView : IHaveBottomAppBar, IPublish
	{
        public AlbumView()
		{
			InitializeComponent();

            Loaded += AlbumViewLoaded;
		}

        public IPresentationBus PresentationBus { get; set; }

		public Type BottomAppBarContentType
		{
			get { return typeof (AlbumViewBottomAppBar); }
		}

        void AlbumViewLoaded(object sender, RoutedEventArgs e)
        {
            var playDropLocationRequest = new PlayDropLocationRequest();
            PresentationBus.Publish(playDropLocationRequest);

            var playlistDropLocationRequest = new PlaylistDropLocationRequest();
            PresentationBus.Publish(playlistDropLocationRequest);

            ((AlbumViewModel)DataContext).SetLocations(playDropLocationRequest.Location, playlistDropLocationRequest.Location);
        }
	}
}
