using System;
using System.Collections.Generic;
using Jukebox.Model;
using Windows.UI.Xaml.Media.Imaging;

namespace Jukebox.Features.Albums.DesignTime
{
    public class DesignTimeAlbumViewModel
    {
        public DesignTimeAlbumViewModel()
        {
            Title = "Album A";
       }

        public string Title { get; set; }

        public BitmapImage LargeBitmap
        {
            get
            {
                var bitmapImage = new BitmapImage
                                      {
                                          UriSource = new Uri("/Images/no_image_large.png", UriKind.RelativeOrAbsolute)
                                      };
                return bitmapImage;
            }
        }
    }

    public class DesignTimeTrackViewModels : List<DesignTimeTrackViewModel>
    {
        public DesignTimeTrackViewModels()
        {
            Add(new DesignTimeTrackViewModel
                    {TrackNumber = 1, Title = "Design Track 1 with a long name", Duration = new TimeSpan(0, 3, 45), DiscNumber = 1});
            Add(new DesignTimeTrackViewModel
                    {TrackNumber = 2, Title = "Design Track 2", Duration = new TimeSpan(0, 2, 30), DiscNumber = 1});

        }
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