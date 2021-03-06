﻿using Jukebox.WinStore.Model;
using PresentationBus;

namespace Jukebox.WinStore.Events
{
    public class PlaylistDataLoaded : PresentationEvent
    {
        public PlaylistDataLoaded(PlaylistData playlistData)
        {
            PlaylistData = playlistData;
        }

        public PlaylistData PlaylistData { get; set; }
    }
}