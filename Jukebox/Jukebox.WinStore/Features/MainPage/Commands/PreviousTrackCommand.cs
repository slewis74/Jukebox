using Windows.Media;
using Jukebox.WinStore.Events;
using Jukebox.WinStore.Requests;
using Orienteer.Xaml.ViewModels;
using Slew.PresentationBus;

namespace Jukebox.WinStore.Features.MainPage.Commands
{
    public class PreviousTrackCommand : 
        PresentationRequestCommand<PreviousTrackRequest>,
        IHandlePresentationEvent<CanMovePreviousChangedEvent>,
        IHandlePresentationEvent<PlaylistDataLoaded>
    {
        private readonly SystemMediaTransportControls _systemMediaTransportControls;
        private bool _canMovePrevious;

        public PreviousTrackCommand(IPresentationBus presentationBus) : base(presentationBus)
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
        }

        public override bool CanExecute(object parameter)
        {
            return _canMovePrevious;
        }

        public void Handle(CanMovePreviousChangedEvent e)
        {
            _systemMediaTransportControls.IsPreviousEnabled = e.CanMovePrevious;
            _canMovePrevious = e.CanMovePrevious;
            RaiseCanExecuteChanged();
        }

        public void Handle(PlaylistDataLoaded presentationEvent)
        {
            _systemMediaTransportControls.IsPreviousEnabled = presentationEvent.PlaylistData.NowPlayingPlaylist.CanMovePrevious;
            _canMovePrevious = presentationEvent.PlaylistData.NowPlayingPlaylist.CanMovePrevious;
            RaiseCanExecuteChanged();
        }
    }
}