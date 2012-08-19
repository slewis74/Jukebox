using Jukebox.Model;
using Windows.Storage;

namespace Jukebox.DataPersistence
{
    public class SongPersister
    {
        private readonly Song _song;

        public SongPersister(Song song)
        {
            _song = song;
            _song.PropertyChanged += (sender, args) => Save(args.PropertyName);
        }

        private void Save(string propertyName)
        {
            var artistsContainer = ApplicationData.Current.LocalSettings.CreateContainer("Artists", ApplicationDataCreateDisposition.Always);

            var album = _song.Album;
            
            var artistContainer = artistsContainer.CreateContainer(album.Artist.Name, ApplicationDataCreateDisposition.Always);
            var albumsContainer = artistContainer.CreateContainer("Albums", ApplicationDataCreateDisposition.Always);
            var albumContainer = albumsContainer.CreateContainer(album.Title, ApplicationDataCreateDisposition.Always);
            var songsContainer = albumContainer.CreateContainer("Songs", ApplicationDataCreateDisposition.Always);

            var trackNumber = _song.TrackNumber.ToString();

            if (songsContainer.Values.Keys.Contains(trackNumber))
            {
                var songComposite = (ApplicationDataCompositeValue)songsContainer.Values[trackNumber];
                switch (propertyName)
                {
                    case "Title":
                        songComposite["Title"] = _song.Title;
                        break;
                    case "TrackNumber":
                        songComposite["TrackNumber"] = _song.TrackNumber;
                        break;
                    case "Path":
                        songComposite["Path"] = _song.Path;
                        break;
                }
            }
            else
            {
                var songComposite = new ApplicationDataCompositeValue();
                songComposite["Title"] = _song.Title;
                songComposite["TrackNumber"] = _song.TrackNumber;
                songComposite["Path"] = _song.Path;

                songsContainer.Values[trackNumber] = songComposite;
            }
        }
    }
}