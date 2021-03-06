﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Model;
using Newtonsoft.Json;
using Serilog;
using Orienteer.Data;
using PresentationBus;

namespace Jukebox.WinStore.Storage
{
    public class MusicProvider : IMusicProvider
    {
        private readonly IPresentationBus _presentationBus;

        public MusicProvider(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;

            Artists = new DistinctAsyncObservableCollection<Artist>();
        }

        public DistinctAsyncObservableCollection<Artist> Artists { get; private set; }

        public async Task LoadContent()
        {
            var artists = new List<Artist>();
            await LoadData(artists);
            
            Artists.StartLargeUpdate();
            Artists.AddRange(artists);
            Artists.CompleteLargeUpdate();

            await _presentationBus.Publish(new AlbumDataLoaded(Artists));
        }

        /// <summary>
        /// Loads the existing content from the application data
        /// </summary>
        private async Task<bool> LoadData(List<Artist> artists)
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            if (files.Any(f => f.Name == "Artists") == false)
                return false;

            try
            {
                var artistsFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Artists", CreationCollisionOption.OpenIfExists);

                var data = await FileIO.ReadTextAsync(artistsFile);
                var artistsData = JsonConvert.DeserializeObject<IEnumerable<Artist>>(data);

                foreach (var artist in artistsData)
                {
                    artists.Add(artist);
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public async Task<bool> ReScanMusicLibrary()
        {
            // copy to a separate list while loaded, to stop the UI flickering when reading lots of new data
            var artists = Artists.ToList();
            Log.Information("Scanning music folder...");
            var newTracks = await ScanMusicLibraryFolder(KnownFolders.MusicLibrary, artists);
            Log.Information("Finished scanning music folder");

            if (newTracks)
            {
                Artists.Replace(artists);
                await SaveData(Artists);
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
                                    Folder = Path.Combine(relativeFolder, fileProps.Album.RemoveIllegalChars())
                                };
                            artist.Albums.Add(album);
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
                            album.Songs.Add(song);
                            newData = true;
                            await _presentationBus.Publish(new SongLoadedEvent(album, song));
                            // save new entry to app storage
                        }
                        song.SetStorageFile(f);
                    }
                }
                newData |= await ScanMusicLibraryFolder(folder, artists);
            }
            return newData;
        }
        
        private async Task SaveData(IEnumerable<Artist> artists)
        {
            var artistsFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Artists", CreationCollisionOption.ReplaceExisting);

            var artistsData = JsonConvert.SerializeObject(artists);

            await FileIO.WriteTextAsync(artistsFile, artistsData);
            
            Debug.WriteLine("Save completed");
        }
    }

    public static class FolderPathStringExtensions
    {
        public static string RemoveIllegalChars(this string @this)
        {
            return
                @this.Replace("<", string.Empty)
                    .Replace(">", string.Empty)
                    .Replace(":", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace("/", string.Empty)
                    .Replace("?", string.Empty)
                    .Replace("|", string.Empty)
                    .Replace("*", string.Empty)
                    .Trim();
        }
    }
}