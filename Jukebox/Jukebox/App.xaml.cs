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
        private DistinctAsyncObservableCollection<Playlist> _playlists;
        private PlaylistHandler _playlistHandler;

        private IContainer _container;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(App).GetTypeInfo().Assembly);
            _container = builder.Build();

            var bus = _container.Resolve<IPresentationBus>();

            // Resolve the SettingsManager, to wire up the settings handlers.
            _container.Resolve<ISettingsManager>();

            var musicProvider = _container.Resolve<IMusicProvider>();
            musicProvider.LoadContent();

            _playlistHandler = _container.Resolve<PlaylistHandler>();
            var playlistData = _playlistHandler.LoadContent();
            _playlists = new DistinctAsyncObservableCollection<Playlist>(playlistData.Playlists);

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            var navigator = _container.Resolve<INavigator>();
            var mainPageViewModel = new MainPageViewModel(bus, navigator, _playlists, playlistData.NowPlayingPlaylist);
            var mainPageView = new MainPageView
                                   {
                                       PresentationBus = bus, 
                                       Navigator = navigator,
                                       ViewLocator = _container.Resolve<IViewLocator>(),
                                       DataContext = mainPageViewModel,
                                   };
            
            bus.Subscribe(mainPageViewModel);
            bus.Subscribe(mainPageView);
            
            Window.Current.Content = mainPageView;
            Window.Current.Activate();

            Task.Factory.StartNew(() => DoBackgroundProcessing(musicProvider));
        }

        private async void DoBackgroundProcessing(IMusicProvider musicProvider)
        {
            await musicProvider.ReScanMusicLibrary();
        }

        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //TODO: Save application state and stop any background activity
        }
    }
}
