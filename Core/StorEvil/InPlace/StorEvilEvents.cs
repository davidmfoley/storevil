using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.InPlace
{
    public class SessionFinishedEvent
    {
    }

    public class MatchFoundEvent
    {
        public MemberInfo Member;      
    }

    public class StoryStartingEvent
    {
        public Story Story;
    }

    public class StorEvilEvents
    {    
        static readonly EventBus _bus = new EventBus(); 
        public static EventBus Bus
        {
            get { return _bus; }
        }
    }

    public class EventBus : IEventBus
    {
        private Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

        public void Register(object handler)
        {
            var allInterfaces = handler.GetType().GetInterfaces();

            var ifaces = allInterfaces.Where(i => i.Name.StartsWith( "IEventHandler`"));

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

    public interface IEventBus
    {
        void Raise<T>(T e);
        void Register(object handler);
    }


    public interface IEventHandler<T1, T2, T3, T4>
    {
        void Handle(T1 eventToHandle);
        void Handle(T2 eventToHandle);
        void Handle(T3 eventToHandle);
        void Handle(T4 eventToHandle);
    }

    public interface IEventHandler<T1, T2, T3>
    {
        void Handle(T1 eventToHandle);
        void Handle(T2 eventToHandle);
        void Handle(T3 eventToHandle);
    }

    public interface IEventHandler<T1, T2> 
    {
        void Handle(T1 eventToHandle);
        void Handle(T2 eventToHandle);
    }
    public interface IEventHandler<T>
    {
        void Handle(T eventToHandle);
    }
}