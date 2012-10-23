using System.Linq;
using System.Threading.Tasks;
using Jukebox.Common;
using Jukebox.Features.MainPage;
using Jukebox.Features.Settings;
using Jukebox.Model;
using Jukebox.Storage;
using Slew.WinRT.Container;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.Pages.Settings;
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
        private SettingsManager _settingsManager;
        private PlaylistHandler _playlistHandler;

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

            _settingsManager = PropertyInjector.Inject(() => new SettingsManager());
            _settingsManager.Add<SettingsController>("PlayerSettings", "Player Settings", c => c.PlayerSettings());

            var musicLibraryHandler = new MusicLibraryHandler();
            _artists = new DistinctAsyncObservableCollection<Artist>(musicLibraryHandler.LoadContent());

            PropertyInjector.Inject(() => _playlistHandler = new PlaylistHandler());
            var playlistData = _playlistHandler.LoadContent(_artists, false);
            _playlists = new DistinctAsyncObservableCollection<Playlist>(playlistData.Playlists);

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            var mainPageViewModel = PropertyInjector.Inject(() => new MainPageViewModel(_artists, _playlists, playlistData.NowPlayingPlaylist));
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
