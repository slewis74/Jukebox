using Jukebox.Model;
using Slew.WinRT.Data;

namespace Jukebox.Features.Artists.DesignTime
{
    public class DesignArtistsViewModel
    {
        private AsyncObservableCollection<GroupedData<Artist>> _groups;

        public AsyncObservableCollection<GroupedData<Artist>> GroupedItems
        {
            get
            {
                if (_groups == null)
                {
                    _groups = new AsyncObservableCollection<GroupedData<Artist>>();
                    
                    var groupedData = new GroupedData<Artist> {Key = "A"};
                    groupedData.Add(new Artist(null) { Name = "Artist A"});
                    _groups.Add(groupedData);
                }
                return _groups;
            }
        }
         
    }
}