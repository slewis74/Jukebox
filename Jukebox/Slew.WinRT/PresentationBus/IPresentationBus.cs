using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Slew.WinRT.PresentationBus
{
    public interface IPresentationBus
    {
        void Subscribe<T>(IPresentationEventHandler<T> handler);
        void UnSubscribe<T>(IPresentationEventHandler<T> handler);

        void Publish<T>(IPresentationEvent<T> presentationEvent);
    }

    public class PresentationBus : IPresentationBus
    {
        private readonly SynchronizationContext _uicontext;
        private readonly List<IPresentationEventHandler> _handlers; 
        
        public PresentationBus()
        {
            _uicontext = SynchronizationContext.Current;
            _handlers = new List<IPresentationEventHandler>();
        }

        public void Subscribe<T>(IPresentationEventHandler<T> handler){}
        public void UnSubscribe<T>(IPresentationEventHandler<T> handler){}

        public void Publish<T>(IPresentationEvent<T> presentationEvent)
        {
        }

        protected void DispatchCall(SendOrPostCallback call)
        {
            if (SynchronizationContext.Current != _uicontext)
            {
                _uicontext.Post(call, null);
            }
            else
            {
                call(null);
            }
        }

        internal class Subscribers<T>
        {
            private List<WeakReference> _subscribers; 

            public Subscribers()
            {
            }

            public void AddSubscriber(IPresentationEventHandler<T> instance)
            {
                if (_subscribers.Any(s => s.Target == instance))
                    return;

                _subscribers.Add(new WeakReference(instance));
            }

            public void RemoveSubscriber(IPresentationEventHandler<T> instance)
            {
                var subscriber = _subscribers.SingleOrDefault(s => s.Target == instance);
                if (subscriber != null)
                {
                    _subscribers.Remove(subscriber);
                }
            }

            public int PublishEvent(IPresentationEvent<T> presentationEvent)
            {
                var numberOfActiveSubscribers = 0;
                foreach (var subscriber in _subscribers.Where(s => s.Target != null))
                {
                    ((IPresentationEventHandler<T>)subscriber.Target).Handle(presentationEvent);
                    numberOfActiveSubscribers++;
                }
                return numberOfActiveSubscribers;
            }
        }
    }
}