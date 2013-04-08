using System.Collections.Generic;
using Jukebox.Model;
using Jukebox.Requests;
using Slab.PresentationBus;
using Slab.ViewModels;

namespace Jukebox.Features.Artists.All
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

        public override void Execute(object parameter)
        {
            _presentationBus.Publish(new PlayAllNowRequest(_artists));
        }
    }
}