using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Jukebox.Common;
using Jukebox.Features.MainPage;
using Jukebox.Features.Settings;
using Jukebox.Model;
using Jukebox.Storage;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
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
        private SettingsHandler _settingsHandler;

        private IContainer _container;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var bus = new PresentationBus();
            var navigator = new Navigator(bus, new JukeboxControllerFactory());

            var builder = new ContainerBuilder();
            builder.RegisterInstance(bus);
            builder.RegisterInstance(navigator);
            builder.RegisterAssemblyModules(typeof(App).GetTypeInfo().Assembly);

            _container = builder.Build();
            
            //PropertyInjector.AddRule(new PublisherInjectorRule(bus));
            //PropertyInjector.AddRule(new SubscriberInjectorRule(bus));
            //PropertyInjector.AddRule(new CanRequestNavigationInjectorRule(navigator));

            _settingsManager = new SettingsManager(navigator);
            _settingsManager.Add<SettingsController>("PlayerSettings", "Player Settings", c => c.PlayerSettings());
            bus.Subscribe(_settingsManager);

            _settingsHandler = new SettingsHandler();
            bool isRandomPlayMode = _settingsHandler.IsGetRandomPlayMode();
            bus.Subscribe(_settingsHandler);

            var musicLibraryHandler = new MusicLibraryHandler(bus);
            _artists = new DistinctAsyncObservableCollection<Artist>(musicLibraryHandler.LoadContent());

            _playlistHandler = new PlaylistHandler(bus);
            var playlistData = _playlistHandler.LoadContent(_artists, isRandomPlayMode);
            _playlists = new DistinctAsyncObservableCollection<Playlist>(playlistData.Playlists);
            bus.Subscribe(_playlistHandler);

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            var mainPageViewModel = new MainPageViewModel(bus, navigator, _artists, _playlists, playlistData.NowPlayingPlaylist);
            var mainPageView = new MainPageView
                                   {
                                       PresentationBus = bus, 
                                       Navigator = navigator,
                                       ViewResolver = new ViewResolver(),
                                       DataContext = mainPageViewModel,
                                   };
            
            bus.Subscribe(mainPageViewModel);
            bus.Subscribe(mainPageView);
            
            Window.Current.Content = mainPageView;
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
