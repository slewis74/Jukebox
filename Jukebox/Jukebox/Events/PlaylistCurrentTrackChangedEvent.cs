﻿using Jukebox.Model;
using Slab.PresentationBus;

namespace Jukebox.Events
{
    public class PlaylistCurrentTrackChangedEvent : PresentationEvent<Playlist>
    {
        public PlaylistCurrentTrackChangedEvent(Playlist data, Song song)
            : base(data)
        {
            Song = song;
        }

        public Song Song { get; set; }
    }
}