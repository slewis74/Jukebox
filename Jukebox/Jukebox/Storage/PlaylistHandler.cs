using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jukebox.Model;
using Slew.WinRT.Container;
using Windows.Storage;

namespace Jukebox.Storage
{
    public class PlaylistHandler
    {
        public Playlist LoadContent(IEnumerable<Artist> artists, IList<Playlist> playlists)
        {
            var currentPlaylist = LoadDataAsync(playlists, artists.ToDictionary(k => k.Name));
            return currentPlaylist;
        }

        private Playlist LoadDataAsync(IList<Playlist> playlists, IDictionary<string, Artist> artists)
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Playlists") == false)
                return null;

            var playlistsContainer = ApplicationData.Current.LocalSettings.Containers["Playlists"];

            foreach (var playlistKey in playlistsContainer.Containers.Keys)
            {
                var playlistContainer = playlistsContainer.Containers[playlistKey];

                var playlist = PropertyInjector.Inject(() => new Playlist((string)playlistContainer.Values["Name"]));

                var songsContainer = playlistContainer.Containers["Songs"];
                foreach (var songKey in songsContainer.Values.Keys.OrderBy(Convert.ToInt32))
                {
                    var songComposite = (ApplicationDataCompositeValue) songsContainer.Values[songKey];
                    var artistName = (string) songComposite["ArtistName"];
                    var albumTitle = (string) songComposite["AlbumTitle"];
                    var trackNumber = (uint) songComposite["TrackNumber"];

                    var artist = artists[artistName];
                    var album = artist.Albums.Single(a => a.Title == albumTitle);
                    var song = album.Songs.Single(s => s.TrackNumber == trackNumber);

                    playlist.Add(song);
                }
                playlist.CurrentTrackIndex = (int?)playlistContainer.Values["CurrentTrackIndex"];

                playlists.Add(playlist);
            }

            var currentPlaylistName = (string)playlistsContainer.Values["CurrentPlaylistName"];

            return playlists.FirstOrDefault(p => p.Name == currentPlaylistName);
        }

        public void SaveData(IEnumerable<Playlist> playlists, Playlist currentPlaylist)
        {
            Task.Factory.StartNew(() => DoSaveData(playlists, currentPlaylist));
        }

        private void DoSaveData(IEnumerable<Playlist> playlists, Playlist currentPlaylist)
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Playlists"))
            {
                //ApplicationData.Current.LocalSettings.DeleteContainer("Playlists");
            }

            var playlistsContainer = ApplicationData.Current.LocalSettings.CreateContainer("Playlists", ApplicationDataCreateDisposition.Always);

            foreach (var playlist in playlists)
            {
                var playlistContainer = playlistsContainer.CreateContainer(playlist.Name, ApplicationDataCreateDisposition.Always);

                playlistContainer.Values["Name"] = playlist.Name;
                playlistContainer.Values["CurrentTrackIndex"] = playlist.CurrentTrackIndex;

                var songsContainer = playlistContainer.CreateContainer("Songs", ApplicationDataCreateDisposition.Always);
                var songIndex = 0;
                foreach (var song in playlist)
                {
                    var songComposite = new ApplicationDataCompositeValue();
                    songComposite["ArtistName"] = song.Album.Artist.Name;
                    songComposite["AlbumTitle"] = song.Album.Title;
                    songComposite["TrackNumber"] = song.TrackNumber;

                    songsContainer.Values[songIndex.ToString()] = songComposite;
                    songIndex++;
                }
            }

            playlistsContainer.Values["CurrentPlaylistName"] = currentPlaylist.Name;
        }
    }
}