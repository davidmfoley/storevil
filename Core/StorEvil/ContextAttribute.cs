using System;

namespace StorEvil
{
   
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ContextAttribute : Attribute
    {
        public ContextLifetime Lifetime { get; set; }
    }

    public enum ContextLifetime
    {
        Scenario,
        Story,
        Run
    }

    public static class ScenarioStatus
    {
        public static void Pending()
        {
            throw new ScenarioPendingException();
        }
    }

    public class ScenarioPendingException : Exception
    {
    }
}
