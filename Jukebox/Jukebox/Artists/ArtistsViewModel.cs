using System;
using System.Linq;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.ViewModels;

namespace Jukebox.Artists
{
    public class ArtistsViewModel : CanRequestNavigationBase
	{
        private readonly DistinctAsyncObservableCollection<Artist> _artists;
	    private AsyncObservableCollection<GroupedData<Artist>> _groups;

        public ArtistsViewModel(DistinctAsyncObservableCollection<Artist> artists)
		{
			_artists = artists;
            _artists.CollectionChanged += (sender, args) => NotifyChanged(() => GroupedItems);

            DisplayArtist = new DisplayArtistCommand(new Lazy<INavigator>(() => Navigator));
		}

		public DisplayArtistCommand DisplayArtist { get; private set; }

        public DistinctAsyncObservableCollection<Artist> Items { get { return _artists; } }

        public AsyncObservableCollection<GroupedData<Artist>> GroupedItems
		{
			get
			{
                if (_groups == null)
                    _groups = new AsyncObservableCollection<GroupedData<Artist>>();
                
                _groups.StartLargeUpdate();
                _groups.Clear();
				var query = from item in _artists
							orderby item.Name
							group item by item.Name.Substring(0, 1) into g
							select new { GroupName = g.Key, Items = g };
				foreach (var g in query)
				{
					var info = new GroupedData<Artist>
					           	{
					           		Key = g.GroupName
					           	};
					info.AddRange(g.Items);
					_groups.Add(info);
				}
                _groups.CompleteLargeUpdate();

				return _groups;
			}
		}
	}
	
	public class DisplayArtistCommand : Command<Artist>
	{
        private readonly Lazy<INavigator> _navigator;

	    public DisplayArtistCommand(Lazy<INavigator> navigator)
        {
            _navigator = navigator;
        }

	    public override void Execute(Artist parameter)
		{
			_navigator.Value.Navigate<ArtistController>(c => c.ShowArtist(parameter));
		}
	}
}