using System;
using System.Collections.Generic;
using System.Linq;
using Jukebox.Events;
using Jukebox.Features.MainPage.Events;
using Jukebox.Requests;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Model
{
    public class NowPlayingPlaylist : Playlist,
        IHandlePresentationEvent<SongEndedEvent>,
        IHandlePresentationRequest<PreviousTrackRequest>,
        IHandlePresentationRequest<NextTrackRequest>
    {
        public const string NowPlayingName = "NowPlaying";

        private readonly bool _isRandomPlayMode;

        public NowPlayingPlaylist(bool isRandomPlayMode)
            : base(NowPlayingName)
        {
            _isRandomPlayMode = isRandomPlayMode;
        }
        public NowPlayingPlaylist(bool isRandomPlayMode, IEnumerable<Song> tracks, int? currentTrackIndex)
            : base(NowPlayingName, tracks)
        {
            _isRandomPlayMode = isRandomPlayMode;
            _currentTrack = currentTrackIndex == null || currentTrackIndex >= Count ? null : this[currentTrackIndex.Value];
        }

        private Song _currentTrack;
        public Song CurrentTrack
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

        protected override void InsertItem(int index, Song item)
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

            int index;
            if (_isRandomPlayMode == false)
            {
                index = IndexOf(_currentTrack) + 1;
            }
            else
            {
                var r = new Random(DateTime.Now.Millisecond);
                index = r.Next(0, Count);
            }
            CurrentTrack = this[index];
        }

        protected override void OnListChanged()
        {
            PresentationBus.Publish(new NowPlayingContentChangedEvent(this, CanMovePrevious, CanMoveNext));
        }

        protected void OnCurrentTrackChanged(Song e)
        {
            PresentationBus.Publish(new NowPlayingCurrentTrackChangedEvent(this, e, CanMovePrevious, CanMoveNext));
        }

        public void Handle(PreviousTrackRequest request)
        {
            MoveToPreviousTrack();
        }
        public void Handle(NextTrackRequest request)
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
    }
}