using System.Threading;
using Slew.WinRT.Pages.Navigation;

namespace Slew.WinRT.ViewModels
{
    public abstract class SearchViewModelBase<TResult> : CanRequestNavigationBase, ISearchViewModelBase
    {
        protected SearchViewModelBase(
            INavigator navigator,
            TResult[] searchResults)
            : base(navigator)
        {
            SearchResults = searchResults;
        }

        protected SearchViewModelBase(
            INavigator navigator, 
            TResult[] searchResults,
            SynchronizationContext synchronizationContext) : base(navigator, synchronizationContext)
        {
            SearchResults = searchResults;
        }

        public override string PageTitle
        {
            get { return "Search Results"; }
        }

        public TResult[] SearchResults { get; set; }
    }
}