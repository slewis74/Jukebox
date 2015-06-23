using System;
using Windows.UI.Xaml;
using Jukebox.WinStore.Requests;
using Orienteer.WinStore.Pages;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Features.Albums
{
	public sealed partial class AlbumView : IHaveBottomAppBar
	{
        public AlbumView(IPresentationBus presentationBus)
		{
            PresentationBus = presentationBus;
            InitializeComponent();

            Loaded += AlbumViewLoaded;
        }

	    public Type BottomAppBarContentType { get { return typeof (AlbumBottomAppBarView); } }

        public IPresentationBus PresentationBus { get; set; }

        async void AlbumViewLoaded(object sender, RoutedEventArgs e)
        {
            var playDropLocationRequest = new PlayDropLocationRequest();
            await PresentationBus.PublishAsync(playDropLocationRequest);

            ((AlbumViewModel)DataContext).SetLocations(playDropLocationRequest.Location);
        }
	}
}
