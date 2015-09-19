using System;
using System.Collections.Generic;
using System.Linq;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Features.MainPage.Events;
using Jukebox.WinStore.Requests;
using PresentationBus;

namespace Jukebox.WinStore.Model
{
    public class NowPlayingPlaylist : Playlist,
        IHandlePresentationEvent<SongEndedEvent>,
        IHandlePresentationCommand<PreviousTrackCommand>,
        IHandlePresentationCommand<NextTrackCommand>,
        IHandlePresentationEvent<RandomPlayModeChangedEvent>,
        IHandlePresentationCommand<PlaySongNowCommand>,
        IHandlePresentationCommand<PlayAlbumNowCommand>,
        IHandlePresentationCommand<PlayArtistNowCommand>,
        IHandlePresentationCommand<PlayAllNowCommand>
    {
        public const string NowPlayingName = "NowPlaying";

        private bool _isRandomPlayMode;

        public delegate NowPlayingPlaylist DefaultFactory(bool isRandomPlayMode);
        public delegate NowPlayingPlaylist WithTracksFactory(bool isRandomPlayMode, IEnumerable<PlaylistSong> tracks, int? currentTrackIndex);

        public NowPlayingPlaylist(IPresentationBus presentationBus, bool isRandomPlayMode)
            : base(presentationBus, NowPlayingName)
        {
            _isRandomPlayMode = isRandomPlayMode;
        }
        public NowPlayingPlaylist(IPresentationBus presentationBus, bool isRandomPlayMode, IEnumerable<PlaylistSong> tracks, int? currentTrackIndex)
            : base(presentationBus, NowPlayingName, tracks)
        {
            _isRandomPlayMode = isRandomPlayMode;
            _currentTrack = currentTrackIndex == null || currentTrackIndex >= Count ? null : this[currentTrackIndex.Value];
        }

        private PlaylistSong _currentTrack;
        public PlaylistSong CurrentTrack
        {
            get { return _currentTrack; }
            set
            {
                _currentTrack = value;
                NotifyChanged(() => CurrentTrack);
                NotifyChanged(() => CurrentTrackIndex);
                OnCurrentTrackChanged(value);
            }
        }

        protected override void InsertItem(int index, PlaylistSong item)
        {
            base.InsertItem(index, item);

            // when first item gets added set it to be the current track.
            if (Count == 1)
            {
                CurrentTrackIndex = 0;
            }
        }

        protected override void ClearItems()
        {
            CurrentTrack = null;
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            int? trackIndexToMoveTo = null;
            var currentTrackIndex = CurrentTrackIndex;
            if (index == currentTrackIndex)
            {
                if (CanMoveNext)
                {
                    trackIndexToMoveTo = currentTrackIndex.Value + 1;
                }
                else if (Count > 1)
                {
                    trackIndexToMoveTo = currentTrackIndex.Value - 1;
                }
                else
                {
                    trackIndexToMoveTo = -1;
                }
            }

            base.RemoveItem(index);

            if (trackIndexToMoveTo.HasValue)
            {
                CurrentTrackIndex = trackIndexToMoveTo == -1 ? null : trackIndexToMoveTo;
            }
        }

        public int? CurrentTrackIndex
        {
            get
            {
                var indexOf = IndexOf(_currentTrack);
                return indexOf == -1 ? (int?)null : indexOf;
            }
            set
            {
                CurrentTrack = value == null || value == -1 ? null : this[value.Value];
            }
        }

        public bool CanMovePrevious
        {
            get
            {
                // Don't allow PreviousTrack in random play mode.
                return _isRandomPlayMode == false && _currentTrack != this.FirstOrDefault();
            }
        }

        public void MoveToPreviousTrack()
        {
            if (CanMovePrevious == false)
                return;

            var currentTrackIndex = IndexOf(_currentTrack);
            CurrentTrack = this[currentTrackIndex - 1];
        }

        public bool CanMoveNext
        {
            get
            {
                // In random play mode, there must still be more than 1 item to be able to move next.
                return (_isRandomPlayMode && Count > 1) || _currentTrack != this.LastOrDefault();
            }
        }

        public void MoveToNextTrack()
        {
            if (CanMoveNext == false)
                return;

            var index = IndexOf(_currentTrack);
            if (_isRandomPlayMode == false)
            {
                index++;
            }
            else
            {
                var originalIndex = index;
                do
                {
                    var r = new Random(DateTime.Now.Millisecond);
                    index = r.Next(0, Count);
                } while (originalIndex == index);
            }
            CurrentTrack = this[index];
        }

        protected async override void OnListChanged()
        {
            await PresentationBus.Publish(new NowPlayingContentChangedEvent(this));
            OnCanMoveChanged();
        }

        protected void OnCurrentTrackChanged(PlaylistSong e)
        {
            PresentationBus.Publish(new NowPlayingCurrentTrackChangedEvent(this, e));
            OnCanMoveChanged();
        }

        private void OnCanMoveChanged()
        {
            PresentationBus.Publish(new CanMovePreviousChangedEvent(this, CanMovePrevious));
            PresentationBus.Publish(new CanMoveNextChangedEvent(this, CanMoveNext));
        }

        public void Handle(RandomPlayModeChangedEvent presentationEvent)
        {
            _isRandomPlayMode = presentationEvent.IsRandomPlayMode;
            OnCanMoveChanged();
        }

        public void Handle(PreviousTrackCommand command)
        {
            MoveToPreviousTrack();
        }
        public void Handle(NextTrackCommand command)
        {
            MoveToNextTrack();
        }

        public void Handle(SongEndedEvent e)
        {
            if (CanMoveNext)
            {
                MoveToNextTrack();
            }
        }

        public void Handle(PlaySongNowCommand command)
        {
            Clear();
            Add(new PlaylistSong { ArtistName = command.ArtistName, Album = command.Album, Song = command.Scope });
            CurrentTrack = this[0];
        }

        public void Handle(PlayAlbumNowCommand command)
        {
            StartLargeUpdate();
            Clear();
            AddAlbum(command.ArtistName, command.Scope);
            CompleteLargeUpdate();

            CurrentTrack = this[0];
        }

        public void Handle(PlayArtistNowCommand command)
        {
            StartLargeUpdate();
            Clear();
            foreach (var album in command.Scope.Albums)
            {
                AddAlbum(command.Scope.Name, album);
            }
            CompleteLargeUpdate();

            CurrentTrack = this[0];
        }

        private void AddAlbum(string artistName, Album album)
        {
            foreach (var song in album.Songs.OrderBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber))
            {
                Add(new PlaylistSong { ArtistName = artistName, Album = album, Song = song });
            }
        }

        public void Handle(PlayAllNowCommand command)
        {
            StartLargeUpdate();
            Clear();
            foreach (var artist in command.Artists)
            {
                foreach (var album in artist.Albums)
                {
                    AddAlbum(artist.Name, album);
                }
            }
            CompleteLargeUpdate();

            CurrentTrack = this[0];
        }
    }
}