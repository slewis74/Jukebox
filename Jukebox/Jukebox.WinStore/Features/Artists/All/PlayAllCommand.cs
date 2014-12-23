using System.Collections.Generic;
using Jukebox.WinStore.Model;
using Jukebox.WinStore.Requests;
using Slab.PresentationBus;
using Slab.ViewModels;

namespace Jukebox.WinStore.Features.Artists.All
{
    public class PlayAllCommand : Command
    {
        private readonly IPresentationBus _presentationBus;
        private readonly IEnumerable<Artist> _artists;

        public PlayAllCommand(IPresentationBus presentationBus, IEnumerable<Artist> artists)
        {
            _presentationBus = presentationBus;
            _artists = artists;
        }

        public async override void Execute(object parameter)
        {
            await _presentationBus.PublishAsync(new PlayAllNowRequest(_artists));
        }
    }
}