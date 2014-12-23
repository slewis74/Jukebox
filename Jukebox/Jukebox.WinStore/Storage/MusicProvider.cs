using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Model;
using Newtonsoft.Json;
using Slab.Data;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Storage
{
    public class MusicProvider : IMusicProvider
    {
        private readonly IPresentationBus _presentationBus;
        private readonly SynchronizationContext _uicontext;

        public MusicProvider(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
            _uicontext = SynchronizationContext.Current;
        }

        public DistinctAsyncObservableCollection<Artist> Artists { get; private set; }

        public void LoadContent()
        {
            var artists = new List<Artist>();
            LoadDataAsync(artists);
            
            Artists = new DistinctAsyncObservableCollection<Artist>(artists);
        }

        /// <summary>
        /// Loads the existing content from the application data
        /// </summary>
        private bool LoadDataAsync(List<Artist> artists)
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Artists") == false)
                return false;

            var artistsContainer = ApplicationData.Current.LocalSettings.Containers["Artists"];

            var data = artistsContainer.Values["Data"].ToString();
            var artistsData = JsonConvert.DeserializeObject<IEnumerable<Artist>>(data);

            foreach (var artist in artistsData)
            {
                artists.Add(artist);
            }

            return true;
        }

        public async Task<bool> ReScanMusicLibrary()
        {
            // copy to a separate list while loaded, to stop the UI flickering when reading lots of new data
            var artists = Artists.ToList();
            var newTracks = await ScanMusicLibraryFolder(KnownFolders.MusicLibrary, artists);
            Debug.WriteLine("Finished scanning music folder");

            if (newTracks)
            {
                Artists.Replace(artists);
                await Task.Factory.StartNew(() => SaveData(Artists));
            }

            return true;
        }

        private async Task<bool> ScanMusicLibraryFolder(StorageFolder parentFolder, IList<Artist> artists)
        {
            var newData = false;
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
                            artist = new Artist
                                            {
                                                Name = fileProps.Artist
                                            };
                            newData = true;
                            artists.Add(artist);
                        }
                        var album = artist.Albums.FirstOrDefault(x =>
                                string.Compare(x.Title, fileProps.Album, StringComparison.CurrentCultureIgnoreCase) == 0);
                        if (album == null)
                        {
                            var relativeFolder = folder.FolderRelativeId.Substring(folder.FolderRelativeId.IndexOf(@"\") + 1);
                            album = new Album
                                {
                                    Title = fileProps.Album,
                                    Folder = Path.Combine(relativeFolder, folder.Name)
                                };
                            newData = true;
                        }

                        uint discNumber = 1;
                        if (f.Name[1] == '-')
                        {
                            discNumber = Convert.ToUInt32(f.Name.Substring(0, 1));
                        }

                        var song = album.Songs.FirstOrDefault(s => s.DiscNumber == discNumber && s.TrackNumber == fileProps.TrackNumber);
                        if (song == null)
                        {
                            song = new Song
                                       {
                                           Title = fileProps.Title,
                                           DiscNumber = discNumber,
                                           TrackNumber = fileProps.TrackNumber,
                                           Path = f.Path,
                                           Duration = fileProps.Duration
                                       };
                            newData = true;
                            await _presentationBus.PublishAsync(new SongLoadedEvent(album, song));
                            // save new entry to app storage
                        }
                        song.SetStorageFile(f);
                    }
                }
                newData |= await ScanMusicLibraryFolder(folder, artists);
            }
            return newData;
        }
        
        private void SaveData(IEnumerable<Artist> artists)
        {
            var artistsContainer = ApplicationData.Current.LocalSettings.CreateContainer("Artists", ApplicationDataCreateDisposition.Always);

            var artistsData = JsonConvert.SerializeObject(artists);
            artistsContainer.Values["Data"] = artistsData;
            
            Debug.WriteLine("Save completed");
        }
    }
}