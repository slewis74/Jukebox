using System.Collections.Generic;
using System.Diagnostics;
using Jukebox.WinStore.Events;
using Slab.Data;
using Slab.PresentationBus;

namespace Jukebox.WinStore.Model
{
    [DebuggerDisplay("Playlist {Name}")]
    public class Playlist : DistinctAsyncObservableCollection<Song>
    {
        public Playlist(IPresentationBus presentationBus, string name)
        {
            PresentationBus = presentationBus;
            Name = name;
        }

        public Playlist(IPresentationBus presentationBus, string name, IEnumerable<Song> tracks) : base(tracks)
        {
            PresentationBus = presentationBus;
            Name = name;
        }

        protected IPresentationBus PresentationBus { get; private set; }
        public string Name { get; set; }

        public override void Add(Song item)
        {
            base.Add(item);
            OnListChanged();
        }

        protected override void SetItem(int index, Song item)
        {
            base.SetItem(index, item);
            OnListChanged();
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);
            OnListChanged();
        }

        protected override void InsertItem(int index, Song item)
        {
            base.InsertItem(index, item);
            OnListChanged();
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            OnListChanged();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            OnListChanged();
        }

        protected virtual void OnListChanged()
        {
            PresentationBus.PublishAsync(new PlaylistContentChangedEvent(this));
        }
    }
}