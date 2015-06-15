namespace Jukebox.WinStore.Model
{
    public class PlaylistSong
    {
        public string ArtistName { get; set; }
        public Album Album { get; set; }
        public string AlbumTitle { get { return Album.Title; } }

        public Song Song { get; set; }

        protected bool Equals(PlaylistSong other)
        {
            return Equals(Song, other.Song);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlaylistSong) obj);
        }

        public override int GetHashCode()
        {
            return (Song != null ? Song.GetHashCode() : 0);
        }
    }
}