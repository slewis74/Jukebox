using System.Linq;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.Search
{
    public class SearchResultsViewModel : SearchViewModelBase<SearchResult>
    {
        public delegate SearchResultsViewModel Factory(string queryText, SearchResult[] searchResults);

        private AsyncObservableCollection<GroupedData<SearchResult>> _groups;

        public SearchResultsViewModel(
            INavigator navigator, 
            string queryText,
            SearchResult[] searchResults) : base(navigator, queryText, searchResults)
        {
        }

        public AsyncObservableCollection<GroupedData<SearchResult>> GroupedItems
        {
            get
            {
                if (_groups == null)
                    _groups = new AsyncObservableCollection<GroupedData<SearchResult>>();

                _groups.StartLargeUpdate();
                _groups.Clear();
                var query = from item in SearchResults
                            orderby item.Type, item.Description
                            group item by item.Type
                            into g
                            select new {GroupType = g.Key, Items = g};
                foreach (var g in query)
                {
                    var info = new GroupedData<SearchResult>
                                   {
                                       Key = g.GroupType.ToString()
                                   };
                    info.AddRange(g.Items);

                    _groups.Add(info);
                }
                _groups.CompleteLargeUpdate();

                return _groups;
            }
        }
    }
}