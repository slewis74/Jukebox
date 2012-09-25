using System.Collections.Generic;

namespace Jukebox.Features.Artists.DesignTime
{
    public class DesignTimeArtistViewModel
    {
        public DesignTimeArtistViewModel()
        {
            Albums = new List<DesignTimeAlbum>
                         {
                             new DesignTimeAlbum { Title = "Album 1" },
                             new DesignTimeAlbum { Title = "Album 2" }
                         };
        }

        public string Name { get { return "Artist A"; } }

        public IEnumerable<DesignTimeAlbum> Albums { get; set; }

        public string SmallBitmap { get { return "/Images/no_image_medium.png"; } }
        public string LargeBitmap { get { return "/Images/no_image_large.png"; } }
    }

    public class DesignTimeAlbum
    {
        public string Title { get; set; }
        public string SmallBitmap { get { return "/Images/no_image_medium.png"; } }
    }
}