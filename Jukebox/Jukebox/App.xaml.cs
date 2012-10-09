﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jukebox.Common;
using Jukebox.Features.MainPage;
using Jukebox.Model;
using Jukebox.Storage;
using Slew.WinRT.Container;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Jukebox
{
    sealed partial class App
    {
        private DistinctAsyncObservableCollection<Artist> _artists;
        private DistinctAsyncObservableCollection<Playlist> _playlists;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var bus = new PresentationBus();
            var navigator = new Navigator(bus, new JukeboxControllerFactory());

            PropertyInjector.AddRule(new PublisherInjectorRule(bus));
            PropertyInjector.AddRule(new SubscriberInjectorRule(bus));
            PropertyInjector.AddRule(new CanRequestNavigationInjectorRule(navigator));

            var musicLibraryHandler = new MusicLibraryHandler();
            _artists = new DistinctAsyncObservableCollection<Artist>(musicLibraryHandler.LoadContent());

            var playlistHandler = new PlaylistHandler();
            var playlists = new List<Playlist>();
            var currentPlaylist = _artists.Any() == false ? null : playlistHandler.LoadContent(_artists, playlists);
            _playlists = new DistinctAsyncObservableCollection<Playlist>(playlists);
            if (_playlists.Any() == false)
            {
                currentPlaylist = PropertyInjector.Inject(() => new Playlist("Default", false));
                _playlists.Add(currentPlaylist);
                playlistHandler.SaveData(_playlists, currentPlaylist);
            }
            else if (currentPlaylist == null)
            {
                currentPlaylist = _playlists.Single(p => p.Name == "Default");
                playlistHandler.SaveData(_playlists, currentPlaylist);
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            var mainPageViewModel = PropertyInjector.Inject(() => new MainPageViewModel(_artists, _playlists, currentPlaylist, playlistHandler));
            Window.Current.Content = PropertyInjector.Inject(() => new MainPageView
        	                             {
                                             DataContext = mainPageViewModel
        	                             });
            Window.Current.Activate();

            Task.Factory.StartNew(() => DoBackgroundProcessing(musicLibraryHandler));
        }

        private async void DoBackgroundProcessing(MusicLibraryHandler musicLibraryHandler)
        {
            await musicLibraryHandler.ReScanMusicLibrary(_artists);
        }

        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //TODO: Save application state and stop any background activity
        }
    }
}
