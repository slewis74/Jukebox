using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.ViewModels
{
    public class PresentationRequestCommand<T> : Command, IPublish
        where T : IPresentationRequest, new()
    {
        public IPresentationBus PresentationBus { get; set; }

        public override void Execute(object parameter)
        {
            PresentationBus.Publish(new T());
        }
    }
}