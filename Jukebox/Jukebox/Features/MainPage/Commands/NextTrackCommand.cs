using Jukebox.Events;
using Jukebox.Requests;
using Slew.WinRT.PresentationBus;
using Slew.WinRT.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class NextTrackCommand : 
        PresentationRequestCommand<NextTrackRequest>,
        IHandlePresentationEvent<CanMoveNextChangedEvent>
    {
        private bool _canMoveNext;

        public NextTrackCommand(bool canMoveNext)
        {
            _canMoveNext = canMoveNext;
        }

        public override bool CanExecute(object parameter)
        {
            return _canMoveNext;
        }

        public void Handle(CanMoveNextChangedEvent e)
        {
            _canMoveNext = e.CanMoveNext;
            RaiseCanExecuteChanged();
        }
    }
}