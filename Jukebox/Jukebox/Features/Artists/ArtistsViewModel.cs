﻿using System;
using System.Linq;
using Jukebox.Model;
using Slew.WinRT.Data;
using Slew.WinRT.Pages.Navigation;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.Artists
{
    public class ArtistsViewModel : ViewModelWithOrientation
	{
        private readonly IPresentationBus _presentationBus;
        private readonly DistinctAsyncObservableCollection<Artist> _artists;
        private AsyncObservableCollection<GroupedData<GroupedArtistViewModel>> _groups;

        public ArtistsViewModel(
            IPresentationBus presentationBus, 
            INavigator navigator,
            DistinctAsyncObservableCollection<Artist> artists) : base(navigator)
		{
            _presentationBus = presentationBus;
            _artists = artists;
            _artists.CollectionChanged += (sender, args) => NotifyChanged(() => GroupedItems);

            DisplayArtist = new DisplayArtistCommand(Navigator);
            PlayAll = new PlayAllCommand(_presentationBus);
		}

        private ArtistsSnappedViewModel _snappedViewModel;
        public override object SnappedViewModel
        {
            get { return _snappedViewModel ?? (_snappedViewModel = new ArtistsSnappedViewModel(_artists, DisplayArtist, PlayAll)); } 
        }

		public DisplayArtistCommand DisplayArtist { get; private set; }
        public PlayAllCommand PlayAll { get; private set; }

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
}