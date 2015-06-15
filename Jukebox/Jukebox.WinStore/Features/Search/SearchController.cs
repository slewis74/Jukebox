using System.Linq;
using Jukebox.WinStore.Common;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Storage;
using Orienteer.Pages.Navigation;

namespace Jukebox.WinStore.Features.Search
{
    public class SearchController : JukeboxController
    {
        private readonly SearchResultsViewModel.Factory _searchViewModelFactory;
        private readonly IMusicProvider _musicProvider;

        public SearchController(
            SearchResultsViewModel.Factory searchViewModelFactory,
            IMusicProvider musicProvider)
        {
            _searchViewModelFactory = searchViewModelFactory;
            _musicProvider = musicProvider;
        }

        public ActionResult DoSearch(string searchText)
        {
            var results = GetSearchResults(searchText);

            return new ViewModelActionResult(() => _searchViewModelFactory(searchText, results));
        }

        public ActionResult SearchForSuggestions(string searchText)
        {
            return new DataActionResult<SearchResult[]>(GetSearchResults(searchText));
        }

        private SearchResult[] GetSearchResults(string searchText)
        {
            var lowerCaseSearchText = searchText.ToUpper();

            var artistResults = _musicProvider.Artists
                                              .Where(a => a.Name.ToUpper().Contains(lowerCaseSearchText))
                                              .Select(a => new SearchResult
                                                               {
                                                                   Type = SearchResultType.Artist,
                                                                   Description = a.Name,
                                                                   SmallBitmapUri = a.SmallBitmapUri,
                                                                   NavigationUri = "Artists/ShowArtist?name=" + a.Name
                                                               });
            var albumResults = _musicProvider.Artists
                                             .SelectMany(artist => artist.Albums
                                                 .Where(album => album.Title.ToUpper().Contains(lowerCaseSearchText))
                                                 .Select(album => new SearchResult
                                                                  {
                                                                      Type = SearchResultType.Album,
                                                                      Description = album.Title,
                                                                      SmallBitmapUri = album.SmallBitmapUri,
                                                                      NavigationUri =
                                                                          string.Format(
                                                                              "Album/ShowAlbum?artistName={0};albumTitle={1}",
                                                                              artist.Name, album.Title)
                                                                  }));
            var songResults = _musicProvider.Artists
                                            .SelectMany(artist => artist.Albums
                                                .SelectMany(album => album.Songs
                                                    .Where(song => song.Title.ToUpper().Contains(lowerCaseSearchText))
                                                    .Select(song => new SearchResult
                                                             {
                                                                 Type = SearchResultType.Song,
                                                                 Description = song.Title,
                                                                 SmallBitmapUri = album.SmallBitmapUri,
                                                                 NavigationUri =
                                                                     string.Format(
                                                                         "Album/ShowAlbum?artistName={0};albumTitle={1}",
                                                                         artist.Name, album.Title)
                                                             })));

            var results = artistResults
                .Concat(albumResults)
                .Concat(songResults)
                .OrderBy(r => r.Description)
                .ToArray();
            return results;
        }
    }
}