using Jukebox.Events;
using Jukebox.Requests;
using Slab.PresentationBus;
using Slab.ViewModels;

namespace Jukebox.Features.MainPage.Commands
{
    public class NextTrackCommand : 
        PresentationRequestCommand<NextTrackRequest>,
        IHandlePresentationEvent<CanMoveNextChangedEvent>
    {
        private bool _canMoveNext;

        public NextTrackCommand(IPresentationBus presentationBus, bool canMoveNext) : base(presentationBus)
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