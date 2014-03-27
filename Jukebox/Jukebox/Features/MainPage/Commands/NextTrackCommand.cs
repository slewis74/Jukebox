using Windows.Media;
using Jukebox.Events;
using Jukebox.Requests;
using Slab.PresentationBus;
using Slab.Xaml.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class NextTrackCommand : 
        PresentationRequestCommand<NextTrackRequest>,
        IHandlePresentationEvent<CanMoveNextChangedEvent>
    {
        private readonly SystemMediaTransportControls _systemMediaTransportControls;
        private bool _canMoveNext;

        public NextTrackCommand(IPresentationBus presentationBus, bool canMoveNext) : base(presentationBus)
        {
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
            _canMoveNext = canMoveNext;
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
    }
}