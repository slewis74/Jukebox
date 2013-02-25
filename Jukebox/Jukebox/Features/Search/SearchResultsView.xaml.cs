using Jukebox.Model;
using Windows.UI.Xaml.Controls;

namespace Jukebox.Features.Search
{
    public sealed partial class SearchResultsView
    {
        public SearchResultsView()
        {
            InitializeComponent();
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var result = e.ClickedItem as SearchResult;
            if (result == null) return;

            var viewModel = (SearchResultsViewModel)DataContext;

            viewModel.Navigator.Navigate(result.NavigationUri);
        }
    }
}
