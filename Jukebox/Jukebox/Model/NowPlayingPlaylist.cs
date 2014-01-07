using System;
using System.Collections.Generic;
using System.Linq;
using Jukebox.Events;
using Jukebox.Features.MainPage.Events;
using Jukebox.Requests;
using Slab.PresentationBus;

namespace Jukebox.Model
{
    public class NowPlayingPlaylist : Playlist,
        IHandlePresentationEvent<SongEndedEvent>,
        IHandlePresentationRequest<PreviousTrackRequest>,
        IHandlePresentationRequest<NextTrackRequest>,
        IHandlePresentationEvent<RandomPlayModeChangedEvent>,
        IHandlePresentationRequest<PlaySongNowRequest>,
        IHandlePresentationRequest<PlayAlbumNowRequest>,
        IHandlePresentationRequest<PlayArtistNowRequest>,
        IHandlePresentationRequest<PlayAllNowRequest>
    {
        public const string NowPlayingName = "NowPlaying";

        private bool _isRandomPlayMode;

        public NowPlayingPlaylist(IPresentationBus presentationBus, bool isRandomPlayMode)
            : base(presentationBus, NowPlayingName)
        {
            _isRandomPlayMode = isRandomPlayMode;

            presentationBus.Subscribe(this);
        }
        public NowPlayingPlaylist(IPresentationBus presentationBus, bool isRandomPlayMode, IEnumerable<Song> tracks, int? currentTrackIndex)
            : base(presentationBus, NowPlayingName, tracks)
        {
            _isRandomPlayMode = isRandomPlayMode;
            _currentTrack = currentTrackIndex == null || currentTrackIndex >= Count ? null : this[currentTrackIndex.Value];

            presentationBus.Subscribe(this);
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
            await PresentationBus.PublishAsync(new NowPlayingContentChangedEvent(this));
            OnCanMoveChanged();
        }

        protected void OnCurrentTrackChanged(Song e)
        {
            PresentationBus.PublishAsync(new NowPlayingCurrentTrackChangedEvent(this, e));
            OnCanMoveChanged();
        }

        private void OnCanMoveChanged()
        {
            PresentationBus.PublishAsync(new CanMovePreviousChangedEvent(this, CanMovePrevious));
            PresentationBus.PublishAsync(new CanMoveNextChangedEvent(this, CanMoveNext));
        }

        public void Handle(RandomPlayModeChangedEvent presentationEvent)
        {
            _isRandomPlayMode = presentationEvent.Data;
            OnCanMoveChanged();
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

        public void Handle(PlaySongNowRequest request)
        {
            request.IsHandled = true;
            Clear();
            Add(request.Scope);
            CurrentTrack = this[0];
        }

        public void Handle(PlayAlbumNowRequest request)
        {
            request.IsHandled = true;
            
            StartLargeUpdate();
            Clear();
            AddAlbum(request.Scope);
            CompleteLargeUpdate();

            CurrentTrack = this[0];
        }

        public void Handle(PlayArtistNowRequest request)
        {
            request.IsHandled = true;
            
            StartLargeUpdate();
            Clear();
            foreach (var album in request.Scope.Albums)
            {
                AddAlbum(album);
            }
            CompleteLargeUpdate();

            CurrentTrack = this[0];
        }

        private void AddAlbum(Album album)
        {
            foreach (var song in album.Songs.OrderBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber))
            {
                Add(song);
            }
        }

        public void Handle(PlayAllNowRequest request)
        {
            request.IsHandled = true;

            StartLargeUpdate();
            Clear();
            foreach (var artist in request.Artists)
            {
                foreach (var album in artist.Albums)
                {
                    AddAlbum(album);
                }
            }
            CompleteLargeUpdate();

            CurrentTrack = this[0];
        }
    }
}