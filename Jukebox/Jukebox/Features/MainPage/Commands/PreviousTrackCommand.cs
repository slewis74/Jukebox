using Jukebox.Events;
using Jukebox.Requests;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class PreviousTrackCommand : 
        PresentationRequestCommand<PreviousTrackRequest>,
        IHandlePresentationEvent<CanMovePreviousChangedEvent>
    {
        private bool _canMovePrevious;

        public PreviousTrackCommand(bool canMovePrevious)
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