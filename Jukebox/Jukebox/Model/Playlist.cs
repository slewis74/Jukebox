using System.Collections.Generic;
using System.Diagnostics;
using Jukebox.Events;
using Slew.WinRT.Container;
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
            PropertyInjector.Inject(() => this); 
        }

        public Playlist(string name, IEnumerable<Song> tracks) : base(tracks)
        {
            Name = name;
            PropertyInjector.Inject(() => this);
        }

        public IPresentationBus PresentationBus { get; set; }

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
            PresentationBus.Publish(new PlaylistContentChangedEvent(this));
        }
    }
}