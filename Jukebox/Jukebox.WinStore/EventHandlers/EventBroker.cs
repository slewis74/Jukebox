using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Slab.PresentationBus;

namespace Jukebox.WinStore.EventHandlers
{
    public class EventBroker : IHandlePresentationEventAsync<PresentationEvent>
    {
        private readonly IComponentContext _context;

        public EventBroker(IComponentContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(PresentationEvent presentationEvent)
        {
            await RaiseAsync(presentationEvent);
        }

        private readonly Dictionary<Type, MethodInfo> _handleMethodCache = new Dictionary<Type, MethodInfo>();

        public async Task RaiseAsync(IPresentationEvent domainEvent)
        {
            var eventType = domainEvent.GetType();
            var eventHandlerType = typeof(IHandlePresentationEventAsync<>).MakeGenericType(eventType);
            var eventHandlerListType = typeof(IEnumerable<>).MakeGenericType(eventHandlerType);
            var eventHandlers = (IEnumerable)_context.Resolve(eventHandlerListType);

            foreach (var handler in eventHandlers)
            {
                var handleMethod = GetHandleMethod(eventType, eventHandlerType);
                var result = handleMethod.Invoke(handler, new object[] { domainEvent });
                if (result is Task)
                {
                    await ((Task)result);
                }
            }
        }

        private MethodInfo GetHandleMethod(Type factType, Type handlerType)
        {
            if (_handleMethodCache.ContainsKey(factType))
                return _handleMethodCache[factType];

            var handleMethod = handlerType.GetTypeInfo()
                .GetDeclaredMethods("HandleAsync")
                .SingleOrDefault(method => method.GetParameters().Any(p => p.ParameterType == factType));

            if (handleMethod == null)
            {
                var errorMessage = string.Format(
                    "No suitable Handle method was found on type {0} for fact type {1}",
                    handlerType.FullName, factType.FullName);

                throw new Exception(errorMessage);
            }

            _handleMethodCache.Add(factType, handleMethod);

            return handleMethod;
        }

    }
}