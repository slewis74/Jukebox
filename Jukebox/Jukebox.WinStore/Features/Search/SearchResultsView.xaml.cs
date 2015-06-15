using Windows.UI.Xaml.Controls;
using Jukebox.WinStore.Model;

namespace Jukebox.WinStore.Features.Search
{
    public sealed partial class SearchResultsView
    {
        public SearchResultsView()
        {
            InitializeComponent();
        }

        private async void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var result = e.ClickedItem as SearchResult;
            if (result == null) return;

            var viewModel = (SearchResultsViewModel)DataContext;

            await viewModel.Navigator.NavigateAsync(result.NavigationUri, false);
        }
    }
}
