using System.Collections.Generic;
using Jukebox.Model;
using Slab.Data;

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

    public class DesignTimeArtist
    {
        public string Name { get; set; }
    }

    public class DesignTimeGroupedItems : List<GroupedData<DesignTimeArtist>> 
    {
        public DesignTimeGroupedItems()
        {
            var designTimeArtists = new GroupedData<DesignTimeArtist>
                                        {
                                            Key = "A"
                                        };
            designTimeArtists.Add(new DesignTimeArtist { Name = "Artist A"});
            designTimeArtists.Add(new DesignTimeArtist { Name = "Artist A1"});
            Add(designTimeArtists);

            designTimeArtists = new GroupedData<DesignTimeArtist>
            {
                Key = "B"
            };
            designTimeArtists.Add(new DesignTimeArtist { Name = "Artist B" });
            designTimeArtists.Add(new DesignTimeArtist { Name = "Artist B1" });
            Add(designTimeArtists);
        }
    }
}