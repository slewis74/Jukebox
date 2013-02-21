using Jukebox.Model;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.Search
{
    public class SearchResultsViewModel : CanRequestNavigationBase
    {
        public delegate SearchResultsViewModel Factory(SearchResult[] searchResults);

        public SearchResultsViewModel(
            INavigator navigator,
            SearchResult[] searchResults) : base(navigator)
        {
            SearchResults = searchResults;
        }

        public SearchResult[] SearchResults { get; set; }
    }
}