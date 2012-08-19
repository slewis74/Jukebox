using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jukebox.Model;
using Jukebox.Storage;
using Slew.WinRT.Data;
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
            var musicLibraryHandler = new MusicLibraryHandler();
            _artists = new DistinctAsyncObservableCollection<Artist>(musicLibraryHandler.LoadContent());

            var playlistHandler = new PlaylistHandler();
            var playlists = new List<Playlist>();
            var currentPlaylist = _artists.Any() == false ? null : playlistHandler.LoadContent(_artists, playlists);
            _playlists = new DistinctAsyncObservableCollection<Playlist>(playlists);
            if (_playlists.Any() == false)
            {
                currentPlaylist = new Playlist("Default");
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

            Window.Current.Content = new MainPage
        	                             {
                                             DataContext = new MainPageViewModel(_artists, _playlists, currentPlaylist, playlistHandler)
        	                             };
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
