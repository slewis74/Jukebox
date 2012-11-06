using System;
using System.Linq;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.Artists
{
    public class ArtistsViewModel : CanRequestNavigationBase
	{
        private readonly DistinctAsyncObservableCollection<Artist> _artists;
        private AsyncObservableCollection<GroupedData<GroupedArtistViewModel>> _groups;

        public ArtistsViewModel(DistinctAsyncObservableCollection<Artist> artists)
		{
			_artists = artists;
            _artists.CollectionChanged += (sender, args) => NotifyChanged(() => GroupedItems);

            DisplayArtist = new DisplayArtistCommand(new Lazy<INavigator>(() => Navigator));
		}

		public DisplayArtistCommand DisplayArtist { get; private set; }

        public DistinctAsyncObservableCollection<Artist> Items { get { return _artists; } }

        public AsyncObservableCollection<GroupedData<GroupedArtistViewModel>> GroupedItems
		{
			get
			{
                if (_groups == null)
                    _groups = new AsyncObservableCollection<GroupedData<GroupedArtistViewModel>>();
                
                _groups.StartLargeUpdate();
                _groups.Clear();
				var query = from item in _artists
							orderby item.Name
							group item by item.Name.Substring(0, 1) into g
							select new { GroupName = g.Key, Items = g };
				foreach (var g in query)
				{
                    var info = new GroupedData<GroupedArtistViewModel>
					           	{
					           		Key = g.GroupName
					           	};
					info.AddRange(g.Items.Select(z => new GroupedArtistViewModel(z)));
				    var size = 2;
                    
				    info[0].HorizontalSize = size;
				    info[0].VerticalSize = size;
					_groups.Add(info);
				}
                _groups.CompleteLargeUpdate();

				return _groups;
			}
		}
	}

    public class DisplayArtistCommand : NavigationCommand<Artist>
	{
	    public DisplayArtistCommand(Lazy<INavigator> navigator) : base(navigator)
        {}

	    public override void Execute(Artist parameter)
		{
			Navigator.Value.Navigate<ArtistController>(c => c.ShowArtist(parameter));
		}
	}
}