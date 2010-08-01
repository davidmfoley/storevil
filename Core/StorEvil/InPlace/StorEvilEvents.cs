using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StorEvil.InPlace
{

    public delegate void MatchFoundHandler(object sender, MatchFoundHandlerArgs args);

    public class MatchFoundHandlerArgs
    {
        public MemberInfo Member;
    }

    public class StorEvilEvents
    {
        public static event MatchFoundHandler OnMatchFound;
        public static void RaiseMatchFound(object sender, MemberInfo info)        
        {
            if (OnMatchFound == null)
                return;

            OnMatchFound(sender, new MatchFoundHandlerArgs { Member = info});
        }
    }


    public class EventBus
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
            
            foreach (var handler in GetHandlersForType(typeof(T)))
            {
               
                var methodInfo = GetHandleMethod<T>(handler);
                if (methodInfo != null)
                    methodInfo.Invoke(handler, new object[] {e});
            }
        }

        private MethodInfo GetHandleMethod<T>(object handler)
        {
            var handleMethods = handler.GetType().GetMethods().Where(m => m.Name == "Handle" &&  m.GetParameters().Count() == 1);
            var chosenMethod = handleMethods.FirstOrDefault(m => m.GetParameters().First().ParameterType == typeof (T));

            return chosenMethod;            
        }
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