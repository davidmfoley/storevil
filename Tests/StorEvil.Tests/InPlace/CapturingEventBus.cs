using System;
using System.Collections.Generic;
using StorEvil.Events;

namespace StorEvil.InPlace
{
    public class CapturingEventBus : MarshalByRefObject, IEventBus
    {
        public List<object> CaughtEvents = new List<object>();

        public void Raise<T1>(T1 e)
        {
            CaughtEvents.Add(e);
        }

        public void Register(object handler)
        {

        }
    }
}