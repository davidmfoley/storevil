namespace StorEvil.Events
{
    public class StorEvilEvents
    {    
        static readonly EventBus _bus = new EventBus(); 
        public static EventBus Bus
        {
            get { return _bus; }
        }
    }
}