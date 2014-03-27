using System;
using System.Collections.Generic;

namespace Jukebox.WinStore.Features.Albums.DesignTime
{
    public class DesignTimeAlbumViewModel
    {
        public DesignTimeAlbumViewModel()
        {
            ArtistName = "Artist A";
            Title = "Album A";

            Tracks = new List<DesignTimeTrackViewModel>
                        {
                            new DesignTimeTrackViewModel { TrackNumber = 1, Title = "Design Track 1 with a long name", Duration = new TimeSpan(0, 3, 45), DiscNumber = 1 },
                            new DesignTimeTrackViewModel { TrackNumber = 2, Title = "Design Track 2", Duration = new TimeSpan(0, 2, 30), DiscNumber = 1 }
                        };
        }

        public string Title { get; set; }
        public string ArtistName { get; set; }

        public string LargeBitmapUri
        {
            get
            {
                return "/Images/no_image_large.png";
            }
        }

        public IEnumerable<DesignTimeTrackViewModel> Tracks { get; set; } 
    }

    public class DesignTimeTrackViewModel
    {
        public DesignTimeTrackViewModel()
        {
            TrackNumber = 1;
            Title = "Design Track 1 with a long name";
            DiscNumber = 1;
            Duration = new TimeSpan(0, 1, 45);
        }

        public uint TrackNumber { get; set; }
        public string Title { get; set; }
        public uint DiscNumber { get; set; }
        public TimeSpan Duration { get; set; }
    }
}