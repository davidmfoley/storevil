using System.Diagnostics;
using StorEvil.InPlace;

namespace StorEvil.Interpreter
{
    public static class DebugTrace
    {
        public static IDebugListener Listener { get; set; }
        [DebuggerStepThrough]
        public static void Trace(string area, string message)
        {
            if (Listener != null)
                Listener.Trace(string.Format("[{0}] - {1}", area, message));
        }

        [DebuggerStepThrough]
        public static void Trace(object area, string message)
        {
            Trace(area.GetType().Name, message);
        }
    }
}