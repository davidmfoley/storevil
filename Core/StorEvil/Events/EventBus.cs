using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Interpreter;
using StorEvil.Utility;

namespace StorEvil.Events
{
    public interface IEventBus
    {
        void Raise<T>(T e);
        void Register(object handler);
    }

    
    public class EventBus : MarshalByRefObject, IEventBus
    {
        public EventBus()
        {
            _handlers = new List<object>();
        }

        private Dictionary<Type, List<object>> _handlersByEvent = new Dictionary<Type, List<object>>();
        private readonly List<object> _handlers;


        public IEnumerable<object> Handlers  
        {
            get { return _handlers;}        
        }

        public void Register(object handler)
        {
            if (_handlers.Contains(handler))
            {
                DebugTrace.Trace("EventBus", "Tried to add a handler of type " + handler.GetType() + " but it was already registered.");
                return;
            }

            _handlers.Add(handler);
            var allInterfaces = handler.GetType().GetInterfaces();

            var handlerType = typeof (IEventHandler<>);
            var namePrefix = handlerType.FullName.Until("`");
            var ifaces = allInterfaces.Where(i => i.FullName.StartsWith(namePrefix));

            foreach (var iface in ifaces)
            {
                foreach (var messageType in iface.GetGenericArguments())
                {
                    var handlersForType = GetHandlersForType(messageType);
                    handlersForType.Add(handler);
                }
            }
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
            foreach (var handler in GetHandlersForType(typeof(T)).Cast<IEventHandler<T>>())
            {
                handler.Handle(e);
                DebugTrace.Trace("EventBus", " ... handled by: " + handler.GetType().FullName);                
            }
        }

    }
}