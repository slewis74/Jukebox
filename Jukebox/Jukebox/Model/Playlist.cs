using System;
using System.Diagnostics;
using System.Linq;
using Jukebox.Events;
using Slew.WinRT.Data;
using Slew.WinRT.PresentationBus;

namespace Jukebox.Model
{
    [DebuggerDisplay("Playlist {Name}")]
    public class Playlist : DistinctAsyncObservableCollection<Song>, IPublish
    {
        public Playlist(string name)
        {
            Name = name;
        }

        public IPresentationBus PresentationBus { get; set; }

        public string Name { get; set; }

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

        public bool CanMovePrevious(bool random)
        {
            // Don't allow PreviousTrack in random play mode.
            return random == false && _currentTrack != this.First();
        }

        public void MoveToPreviousTrack(bool random)
        {
            if (CanMovePrevious(random) == false)
                return;

            var currentTrackIndex = IndexOf(_currentTrack);
            CurrentTrack = this[currentTrackIndex - 1];
        }

        public bool CanMoveNext(bool random)
        {
            return random || _currentTrack != this.LastOrDefault();
        }

        public void MoveToNextTrack(bool random)
        {
            if (CanMoveNext(random) == false)
                return;
            
            int index;
            if (random == false)
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
            base.ClearItems();
            CurrentTrack = null;
        }

        protected override void RemoveItem(int index)
        {
            int? trackIndexToMoveTo = null;
            var currentTrackIndex = CurrentTrackIndex;
            if (index == currentTrackIndex)
            {
                if (CanMoveNext(false))
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

        public void OnCurrentTrackChanged(Song e)
        {
            PresentationBus.Publish(new CurrentTrackChangedEvent(e));
        }
    }
}