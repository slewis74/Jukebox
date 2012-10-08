using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Container
{
    public class PublisherInjectorRule : IPropertyInjectorRule
    {
        private readonly IPresentationBus _presentationBus;

        public PublisherInjectorRule(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public void Process<T>(T obj)
        {
            var publish = obj as IPublish;
            if (publish != null)
            {
                publish.PresentationBus = _presentationBus;
            }
        }
    }
}