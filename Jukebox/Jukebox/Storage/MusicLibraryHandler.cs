using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jukebox.Model;
using Slew.WinRT.Data;
using Windows.Storage;
using Windows.Storage.Search;

namespace Jukebox.Storage
{
    public class MusicLibraryHandler
    {
        private readonly SynchronizationContext _uicontext;
        public MusicLibraryHandler()
        {
            _uicontext = SynchronizationContext.Current;
        }

        public IEnumerable<Artist> LoadContent()
        {
            var artists = new List<Artist>();
            LoadDataAsync(artists);
            return artists;
        }

        /// <summary>
        /// Loads the existing content from the application data
        /// </summary>
        private bool LoadDataAsync(List<Artist> artists)
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Artists") == false)
                return false;

            var artistsContainer = ApplicationData.Current.LocalSettings.Containers["Artists"];

            foreach (var artistKey in artistsContainer.Containers.Keys.OrderBy(x => x))
            {
                var artistContainer = artistsContainer.Containers[artistKey];
                var artist = new Artist(_uicontext)
                                 {
                                     Name = (string)artistContainer.Values["Name"]
                                 };

                var albumsContainer = artistContainer.Containers["Albums"];
                foreach (var albumContainerKey in albumsContainer.Containers.Keys.OrderBy(x => x))
                {
                    var albumContainer = albumsContainer.Containers[albumContainerKey];
                    var album = new Album(_uicontext)
                                    {
                                        Title = (string)albumContainer.Values["Title"],
                                        Artist = artist,
                                    };

                    var songsContainer = albumContainer.Containers["Songs"];
                    foreach (var songKey in songsContainer.Values.Keys.OrderBy(Convert.ToInt32))
                    {
                        var songComposite = (ApplicationDataCompositeValue)songsContainer.Values[songKey];
                        var song = new Song(_uicontext)
                                       {
                                           DiscNumber = (uint)songComposite["DiscNumber"],
                                           TrackNumber = (uint)songComposite["TrackNumber"],
                                           Title = (string)songComposite["Title"],
                                           Path = (string)songComposite["Path"],
                                           Album = album,
                                           Duration = new TimeSpan(songComposite.ContainsKey("Duration") ? (long)songComposite["Duration"] : 0)
                                       };
                    }
                }
                artists.Add(artist);
            }

            return true;
        }

        public async Task<bool> ReScanMusicLibrary(AsyncObservableCollection<Artist> artists)
        {
            var newArtists = await ScanMusicLibraryFolder(KnownFolders.MusicLibrary, artists.ToList());
            Debug.WriteLine("Finished scanning music folder");

            if (newArtists.Any())
            {
                artists.AddRange(newArtists);
                
                Task.Factory.StartNew(() => SaveData(artists));
            }

            return true;
        }

        private async Task<List<Artist>> ScanMusicLibraryFolder(StorageFolder parentFolder, IList<Artist> artists)
        {
            var newArtists = new List<Artist>();
            foreach (var folder in await parentFolder.GetFoldersAsync())
            {
                foreach (var f in await folder.GetFilesAsync(CommonFileQuery.OrderByMusicProperties))
                {
                    var fileProps = await f.Properties.GetMusicPropertiesAsync();

                    if (string.IsNullOrWhiteSpace(fileProps.Artist) == false)
                    {
                        var artist = artists.FirstOrDefault(x => string.Compare(x.Name, fileProps.Artist, StringComparison.CurrentCultureIgnoreCase) == 0);
                        if (artist == null)
                        {
                            artist = newArtists.FirstOrDefault(x => string.Compare(x.Name, fileProps.Artist, StringComparison.CurrentCultureIgnoreCase) == 0);
                            if (artist == null)
                            {
                                artist = new Artist(_uicontext)
                                             {
                                                 Name = fileProps.Artist
                                             };
                                newArtists.Add(artist);
                                artists.Add(artist);
                            }
                        }
                        var album = artist.Albums.FirstOrDefault(x => string.Compare(x.Title, fileProps.Album, StringComparison.CurrentCultureIgnoreCase) == 0 && x.Artist == artist) ??
                                    new Album(_uicontext)
                                    {
                                        Title = fileProps.Album,
                                        Artist = artist
                                    };

                        uint discNumber = 1;
                        if (f.Name[1] == '-')
                        {
                            discNumber = Convert.ToUInt32(f.Name.Substring(0, 1));
                        }

                        var song = album.Songs.FirstOrDefault(s => s.DiscNumber == discNumber && s.TrackNumber == fileProps.TrackNumber);
                        if (song == null)
                        {
                            song = new Song(_uicontext)
                                       {
                                           Title = fileProps.Title,
                                           DiscNumber = discNumber,
                                           TrackNumber = fileProps.TrackNumber,
                                           Path = f.Path,
                                           Album = album,
                                           Duration = fileProps.Duration
                                       };
                            // save new entry to app storage
                        }
                        song.SetStorageFile(f);
                    }
                }
                newArtists.AddRange(await ScanMusicLibraryFolder(folder, artists));
            }
            return newArtists;
        }
        
        private void SaveData(IEnumerable<Artist> artists)
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Artists"))
            {
                ApplicationData.Current.LocalSettings.DeleteContainer("Artists");
            }

            var artistsContainer = ApplicationData.Current.LocalSettings.CreateContainer("Artists", ApplicationDataCreateDisposition.Always);

            foreach (var artist in artists)
            {
                var artistContainer = artistsContainer.CreateContainer(artist.Name, ApplicationDataCreateDisposition.Always);

                artistContainer.Values["Name"] = artist.Name;

                var albumsContainer = artistContainer.CreateContainer("Albums", ApplicationDataCreateDisposition.Always);

                foreach (var album in artist.Albums)
                {
                    var albumContainer = albumsContainer.CreateContainer(album.Title, ApplicationDataCreateDisposition.Always);

                    albumContainer.Values["Title"] = album.Title;

                    var songsContainer = albumContainer.CreateContainer("Songs", ApplicationDataCreateDisposition.Always);
                    foreach (var song in album.Songs)
                    {
                        var songComposite = new ApplicationDataCompositeValue();
                        songComposite["Title"] = song.Title;
                        songComposite["DiscNumber"] = song.DiscNumber;
                        songComposite["TrackNumber"] = song.TrackNumber;
                        songComposite["Path"] = song.Path;
                        songComposite["Duration"] = song.Duration.Ticks;

                        songsContainer.Values[song.TrackNumber.ToString()] = songComposite;
                    }
                }
            }
            
            Debug.WriteLine("Save completed");
        }
    }
}