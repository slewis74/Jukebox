using Jukebox.Model;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Requests
{
    public class PlayNowRequest<T> : PresentationRequest
    {
        public T Scope { get; set; }
    }

    public class PlaySongNowRequest : PlayNowRequest<Song>
    {}
    public class PlayAlbumNowRequest : PlayNowRequest<Album>
    {}
    public class PlayArtistNowRequest : PlayNowRequest<Artist>
    {}
    public class PlayAllNowRequest : PresentationRequest
    { }
}