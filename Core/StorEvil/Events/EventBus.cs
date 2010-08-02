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
        private Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

        public void Register(object handler)
        {
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
            if (!_handlers.ContainsKey(messageType))
                _handlers.Add(messageType, new List<object>());

            return _handlers[messageType];
        }

        public void Raise<T>(T e)
        {
            DebugTrace.Trace("EventBus", "Raising " + typeof(T).FullName);
            foreach (var handler in GetHandlersForType(typeof(T)))
            {
                var methodInfo = GetHandleMethod<T>(handler);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(handler, new object[] {e});
                    DebugTrace.Trace("EventBus", " ... handled by: " + handler.GetType().FullName);
                }
            }
        }

        private MethodInfo GetHandleMethod<T>(object handler)
        {
            var handleMethods = handler.GetType().GetMethods().Where(m => m.Name == "Handle" &&  m.GetParameters().Count() == 1);
            var chosenMethod = handleMethods.FirstOrDefault(m => m.GetParameters().First().ParameterType == typeof (T));

            return chosenMethod;            
        }
    }
}