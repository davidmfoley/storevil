using System;
using StorEvil.Context;
using StorEvil.Interpreter;

namespace StorEvil.Events
{
    public class EventBusAutoRegistrar
    {
        private readonly AssemblyRegistry _assemblyRegistry;

        public EventBusAutoRegistrar(AssemblyRegistry assemblyRegistry)
        {
            _assemblyRegistry = assemblyRegistry;
        }

        public void InstallTo(IEventBus bus)
        {
            foreach (var type in _assemblyRegistry.GetTypesImplementing(typeof(IHandle<>)))
            {
                try
                {
                    bus.Register(Activator.CreateInstance(type, new object[0]));
                }
                catch(Exception ex)
                {
                    var message = "warning: could not instatiate handler of type " + type + ":\r\n" + ex;
                    DebugTrace.Trace("EventBusAutoRegistrar", message);
                }
            }
        }
    }
}