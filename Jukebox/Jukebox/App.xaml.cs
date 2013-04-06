﻿using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Jukebox.Features.MainPage;
using Jukebox.Features.Search;
using Jukebox.Model;
using Jukebox.Storage;
using Slew.WinRT.Data;
using Slew.WinRT.Data.Navigation;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.Pages.Settings;
using Slew.WinRT.PresentationBus;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
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

            var mainPageViewModel = _container.Resolve<MainPageViewModel>( new Parameter[]
                                                                               {
                                                                                   new NamedParameter("playlists", _playlists), 
                                                                                   new NamedParameter("currentPlaylist", playlistData.NowPlayingPlaylist), 
                                                                                   new NamedParameter("defaultRoute", "Artists/ShowAll"), 
                                                                               });
            var mainPageView = new MainPageView
                                   {
                                       DataContext = mainPageViewModel
                                   };
            
            bus.Subscribe(mainPageView);
            
            Window.Current.Content = mainPageView;
            Window.Current.Activate();

            Task.Factory.StartNew(() => DoBackgroundProcessing(musicProvider));

            var searchPane = SearchPane.GetForCurrentView();
            searchPane.SuggestionsRequested += SearchPaneOnSuggestionsRequested;
        }

        private void SearchPaneOnSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            var navigator = _container.Resolve<INavigator>();

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
