using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
using Windows.UI.Xaml;
using Autofac;
using Jukebox.WinStore.EventHandlers;
using Jukebox.WinStore.Features.MainPage;
using Jukebox.WinStore.Features.Search;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Storage;
using Slab.Pages.Navigation;
using Slab.WinStore.Data.Navigation;
using Slab.WinStore.Host;
using Slab.WinStore.Pages.Navigation;
using Slab.WinStore.Pages.Settings;

namespace Jukebox.WinStore
{
    sealed partial class App
    {
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

            var eb = _container.Resolve<EventBroker>();

            if (string.IsNullOrWhiteSpace(args.Arguments) == false)
            {
                var stackStorage = _container.Resolve<INavigationStackStorage>();
                stackStorage.LaunchingDeepLink(args.Arguments);
            }

            // Resolve the SettingsManager, to wire up the settings handlers.
            _container.Resolve<ISettingsManager>();
            _container.Resolve<PlaylistHandler>();

            // The main page view model needs to be listening for the data loaded events.
            var mainPageViewModel = _container.Resolve<JukeboxHostViewModel>();
            _container.InjectUnsetProperties(mainPageViewModel);

            DoStartup(args, mainPageViewModel);
        }

        private async Task DoStartup(LaunchActivatedEventArgs args, JukeboxHostViewModel mainPageViewModel)
        {
            var musicProvider = _container.Resolve<IMusicProvider>();
            await musicProvider.LoadContent();

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            var nowPlayingHeaderView = _container.Resolve<NowPlayingHeaderView>();
            var mainPageView = new HostView
            {
                HeaderContent = nowPlayingHeaderView,
                DataContext = mainPageViewModel
            };
            Window.Current.Content = mainPageView;
            Window.Current.Activate();

            Task.Factory.StartNew(() => DoBackgroundProcessing(musicProvider));

            var searchPane = SearchPane.GetForCurrentView();
            searchPane.SuggestionsRequested += SearchPaneOnSuggestionsRequested;
        }

        private void SearchPaneOnSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            var navigator = _container.Resolve<IRtNavigator>();

            var result = navigator.GetData<SearchController, SearchResult[]>(c => c.SearchForSuggestions(args.QueryText));

            args.Request.SearchSuggestionCollection.AppendQuerySuggestions(
                result.Data
                .OrderBy(r => r.Description)
                .Select(r => r.Description)
                .Distinct());
        }

        private async void DoBackgroundProcessing(IMusicProvider musicProvider)
        {
            await musicProvider.ReScanMusicLibrary();
        }

        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //TODO: Save application state and stop any background activity
        }

        protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            var navigator = _container.Resolve<INavigator>();

            navigator.Navigate<SearchController>(c => c.DoSearch(args.QueryText));
            
            base.OnSearchActivated(args);
        }
    }
}
