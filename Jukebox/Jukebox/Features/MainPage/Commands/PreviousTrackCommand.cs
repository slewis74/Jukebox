using Jukebox.Events;
using Jukebox.Requests;
using Slab.PresentationBus;
using Slab.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class PreviousTrackCommand : 
        PresentationRequestCommand<PreviousTrackRequest>,
        IHandlePresentationEvent<CanMovePreviousChangedEvent>
    {
        private bool _canMovePrevious;

        public PreviousTrackCommand(IPresentationBus presentationBus, bool canMovePrevious) : base(presentationBus)
        {
            _canMovePrevious = canMovePrevious;
        }

        public override bool CanExecute(object parameter)
        {
            return _canMovePrevious;
        }

        public void Handle(CanMovePreviousChangedEvent e)
        {
            _canMovePrevious = e.CanMovePrevious;
            RaiseCanExecuteChanged();
        }
    }
}