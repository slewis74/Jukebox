using Windows.Media;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Requests;
using Orienteer.Xaml.ViewModels;
using PresentationBus;

namespace Jukebox.WinStore.Features.MainPage.Commands
{
    public class NextTrackCommand : 
        PresentationCommandSenderCommand<WinStore.Requests.NextTrackCommand>,
        IHandlePresentationEvent<CanMoveNextChangedEvent>,
        IHandlePresentationEvent<PlaylistDataLoaded>
    {
        private readonly SystemMediaTransportControls _systemMediaTransportControls;
        private bool _canMoveNext;

        public NextTrackCommand(IPresentationBus presentationBus) : base(presentationBus)
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
        }

        public override bool CanExecute(object parameter)
        {
            return _canMoveNext;
        }

        public void Handle(CanMoveNextChangedEvent e)
        {
            _systemMediaTransportControls.IsNextEnabled = e.CanMoveNext;
            _canMoveNext = e.CanMoveNext;
            RaiseCanExecuteChanged();
        }

        public void Handle(PlaylistDataLoaded presentationEvent)
        {
            _systemMediaTransportControls.IsNextEnabled = presentationEvent.PlaylistData.NowPlayingPlaylist.CanMoveNext;
            _canMoveNext = presentationEvent.PlaylistData.NowPlayingPlaylist.CanMoveNext;
            RaiseCanExecuteChanged();
        }
    }
}