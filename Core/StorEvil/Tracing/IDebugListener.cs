using System;

namespace StorEvil.InPlace
{
    public interface IDebugListener
    {
        void Trace(string message);
    }

    public class NoOpDebugListener : IDebugListener
    {
        public void Trace(string message)
        {
            
        }
    }

    public class ConsoleDebugListener : IDebugListener
    {
        public void Trace(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}