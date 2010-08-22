using System;

namespace StorEvil.Events
{
    public class StorEvilEvents
    {    
        static  EventBus _bus = new EventBus(); 
        public static EventBus Bus
        {
            get { return _bus; }
        }

        public static void SetBus(EventBus eventBus)
        {
            _bus = eventBus;
        }

        public static void ResetBus()
        {
            _bus.Dispose();
            _bus = new EventBus();
        }
    }
}