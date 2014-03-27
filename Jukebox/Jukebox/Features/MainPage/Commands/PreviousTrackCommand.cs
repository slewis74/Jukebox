using Windows.Media;
using Jukebox.Events;
using Jukebox.Requests;
using Slab.PresentationBus;
using Slab.Xaml.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class PreviousTrackCommand : 
        PresentationRequestCommand<PreviousTrackRequest>,
        IHandlePresentationEvent<CanMovePreviousChangedEvent>
    {
        private readonly SystemMediaTransportControls _systemMediaTransportControls;
        private bool _canMovePrevious;

        public PreviousTrackCommand(IPresentationBus presentationBus, bool canMovePrevious) : base(presentationBus)
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
            _canMovePrevious = canMovePrevious;
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
    }
}