using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Model;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Storage
{
    public class PlaylistHandler :
        IHandlePresentationEvent<PlaylistContentChangedEvent>,
        IHandlePresentationEvent<NowPlayingContentChangedEvent>,
        IHandlePresentationEvent<NowPlayingCurrentTrackChangedEvent>,
        IHandlePresentationEventAsync<AlbumDataLoaded>
    {
        private readonly IPresentationBus _presentationBus;
        private readonly ISettingsHandler _settingsHandler;

        public PlaylistHandler(
            IPresentationBus presentationBus,
            ISettingsHandler settingsHandler)
        {
            _presentationBus = presentationBus;
            _settingsHandler = settingsHandler;
        }

        private PlaylistData LoadData(IDictionary<string, Artist> artists, bool isRandomPlayMode)
        {
            var playlists = new List<Playlist>();

            var playlistContainer = ApplicationData.Current.LocalSettings.CreateContainer(NowPlayingPlaylist.NowPlayingName, ApplicationDataCreateDisposition.Always);

            var playlistData = new PlaylistData(_presentationBus, isRandomPlayMode, LoadPlaylist(artists, playlistContainer), (int?)playlistContainer.Values["CurrentTrackIndex"]);

            var playlistsContainer = ApplicationData.Current.LocalSettings.CreateContainer("Playlists", ApplicationDataCreateDisposition.Always);
            foreach (var playlistKey in playlistsContainer.Containers.Keys)
            {
                playlistContainer = playlistsContainer.Containers[playlistKey];

                var playlist = new Playlist(_presentationBus, (string)playlistContainer.Values["Name"], LoadPlaylist(artists, playlistContainer));

                playlists.Add(playlist);
            }

            playlistData.Playlists = playlists;

            return playlistData;
        }

        private static IEnumerable<PlaylistSong> LoadPlaylist(IDictionary<string, Artist> artists, ApplicationDataContainer playlistContainer)
        {
            if (playlistContainer.Containers.ContainsKey("Songs") == false)
                return Enumerable.Empty<PlaylistSong>();

            var songs = new List<PlaylistSong>();

            var songsContainer = playlistContainer.Containers["Songs"];
            foreach (var songKey in songsContainer.Values.Keys.OrderBy(Convert.ToInt32))
            {
                var songComposite = (ApplicationDataCompositeValue) songsContainer.Values[songKey];
                var artistName = (string) songComposite["ArtistName"];
                var albumTitle = (string) songComposite["AlbumTitle"];
                var discNumber = (uint) songComposite["DiscNumber"];
                var trackNumber = (uint) songComposite["TrackNumber"];

                var artist = artists[artistName];
                var album = artist.Albums.Single(a => a.Title == albumTitle);
                var song = album.Songs.SingleOrDefault(s => s.DiscNumber == discNumber && s.TrackNumber == trackNumber);
                if (song != null)
                {
                    songs.Add(new PlaylistSong { ArtistName = artistName, Album = album, Song = song });
                }
                else
                {
                    Debug.WriteLine("Unable to locate playlist track Disc {0}, Track {1} on album {2}", discNumber,
                                    trackNumber, albumTitle);
                }
            }
            return songs;
        }

        public void Handle(NowPlayingCurrentTrackChangedEvent presentationEvent)
        {
            Task.Factory.StartNew(() => DoSaveCurrentTrackForPlaylist((NowPlayingPlaylist)presentationEvent.Playlist));
        }

        private void DoSaveCurrentTrackForPlaylist(NowPlayingPlaylist playlist)
        {
            var playlistContainer = ApplicationData.Current.LocalSettings.CreateContainer(playlist.Name, ApplicationDataCreateDisposition.Always);
            playlistContainer.Values["CurrentTrackIndex"] = playlist.CurrentTrackIndex;
        }

        public void Handle(PlaylistContentChangedEvent presentationEvent)
        {
            Task.Factory.StartNew(() => DoSaveData(presentationEvent.Playlist));
        }

        private void DoSaveData(Playlist playlist)
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Playlists"))
            {
                //ApplicationData.Current.LocalSettings.DeleteContainer("Playlists");
            }

            var playlistsContainer = ApplicationData.Current.LocalSettings.CreateContainer("Playlists", ApplicationDataCreateDisposition.Always);

            var playlistContainer = playlistsContainer.CreateContainer(playlist.Name, ApplicationDataCreateDisposition.Always);

            DoSavePlaylistData(playlist.Name, playlist.ToArray(), playlistContainer);
        }

        public void Handle(NowPlayingContentChangedEvent presentationEvent)
        {
            Task.Factory.StartNew(() => DoSaveData((NowPlayingPlaylist)presentationEvent.Playlist));
        }
        private void DoSaveData(NowPlayingPlaylist playlist)
        {
            var playlistContainer = ApplicationData.Current.LocalSettings.CreateContainer(playlist.Name, ApplicationDataCreateDisposition.Always);

            DoSavePlaylistData(playlist.Name, playlist.ToArray(), playlistContainer);
        }

        private static void DoSavePlaylistData(string playlistName, IEnumerable<PlaylistSong> playlist, ApplicationDataContainer playlistContainer)
        {
            playlistContainer.Values["Name"] = playlistName;

            var songsContainer = playlistContainer.CreateContainer("Songs", ApplicationDataCreateDisposition.Always);
            songsContainer.Values.Clear();
            var songIndex = 0;
            foreach (var playlistSong in playlist)
            {
                var songComposite = new ApplicationDataCompositeValue();
                songComposite["ArtistName"] = playlistSong.ArtistName;
                songComposite["AlbumTitle"] = playlistSong.AlbumTitle;
                songComposite["DiscNumber"] = playlistSong.Song.DiscNumber;
                songComposite["TrackNumber"] = playlistSong.Song.TrackNumber;

                songsContainer.Values[songIndex.ToString()] = songComposite;
                songIndex++;
            }
        }

        public async Task HandleAsync(AlbumDataLoaded presentationEvent)
        {
            var playlistData = LoadData(presentationEvent.Artists.ToDictionary(k => k.Name), _settingsHandler.IsGetRandomPlayMode());

            await _presentationBus.PublishAsync(new PlaylistDataLoaded(playlistData));
        }
    }
}