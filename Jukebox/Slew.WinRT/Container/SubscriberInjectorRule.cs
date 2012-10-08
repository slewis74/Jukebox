using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Container
{
    public class SubscriberInjectorRule : IPropertyInjectorRule
    {
        private readonly IPresentationBus _presentationBus;

        public SubscriberInjectorRule(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public void Process<T>(T obj)
        {
            var handlesEvents = obj as IHandlePresentationEvents;
            if (handlesEvents != null)
            {
                _presentationBus.Subscribe(handlesEvents);
            }
        }
    }
}