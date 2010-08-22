using System;
using System.Collections.Generic;
using System.Linq;
using StorEvil.Interpreter;

namespace StorEvil.Events
{
    public interface IEventBus
    {
        void Raise<T>(T e);
        void Register(object handler);
    }

    
    public class EventBus : MarshalByRefObject, IEventBus, IDisposable
    {      
        private Dictionary<Type, List<object>> _handlersByEvent = new Dictionary<Type, List<object>>();
        private readonly List<object> _handlers = new List<object>();

        public IEnumerable<object> Handlers  
        {
            get { return _handlers;}        
        }

        public void Register(object handler)
        {
            if (IsAlreadyRegistered(handler))
            {
                DebugTrace.Trace("EventBus", "Tried to add a handler of type " + handler.GetType() + " but it was already registered.");
                return;
            }

            _handlers.Add(handler);

            AssociateWithHandledEventTypes(handler);
        }

        private bool IsAlreadyRegistered(object handler)
        {
            return _handlers.Contains(handler);
        }

        private void AssociateWithHandledEventTypes(object handler)
        {
            foreach (var messageType in GetHandledEventTypes(handler))
            {
                var handlersForType = GetHandlersForType(messageType);
                handlersForType.Add(handler);
            }
        }       

        private static IEnumerable<Type> GetHandledEventTypes(object handler)
        {
            var handlerType = typeof (IHandle<>);
            var allInterfaces = handler.GetType().GetInterfaces();
            var handlerInterfaces = allInterfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);
            return handlerInterfaces.Select(x => x.GetGenericArguments().First());
        }

        private List<object> GetHandlersForType(Type messageType)
        {
            if (!_handlersByEvent.ContainsKey(messageType))
                _handlersByEvent.Add(messageType, new List<object>());

            return _handlersByEvent[messageType];
        }

        public void Raise<T>(T e)
        {
            DebugTrace.Trace("EventBus", "Raising " + typeof(T).FullName);
            foreach (var handler in GetHandlersForType(typeof(T)).Cast<IHandle<T>>())
            {
                handler.Handle(e);
                DebugTrace.Trace("EventBus", " ... handled by: " + handler.GetType().FullName);                
            }
        }

        public void Dispose()
        {
            foreach (var handler in Handlers)
            {
                if (handler is IDisposable)
                    ((IDisposable)handler).Dispose();
            }

            _handlers.Clear();
        }
    }
}