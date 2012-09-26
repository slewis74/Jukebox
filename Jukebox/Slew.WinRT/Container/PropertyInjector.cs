using System;
using Slew.WinRT.PresentationBus;

namespace Slew.WinRT.Container
{
    public static class PropertyInjector
    {
        public static IPresentationBus PresentationBus { get; set; }

        public static T Inject<T>(Func<T> objectCreationAction)
        {
            var obj = objectCreationAction();

            var publish = obj as IPublish;
            if (publish != null)
            {
                publish.PresentationBus = PresentationBus;
            }

            var handlesEvents = obj as IHandlePresentationEvents;
            if (handlesEvents != null)
            {
                PresentationBus.Subscribe(handlesEvents);
            }

            return obj;
        }
    }
}