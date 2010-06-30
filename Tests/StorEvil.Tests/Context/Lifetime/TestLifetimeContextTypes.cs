using System;

namespace StorEvil.Context.Lifetime
{
    [Context(Lifetime = ContextLifetime.Scenario)]
    public class ScenarioLifetimeTestMappingContext
    {
    }

    [Context(Lifetime = ContextLifetime.Session)]
    public class TestSessionLifetimeMappingContext : IDisposable
    {
        public static bool WasDisposed;

        public TestSessionLifetimeMappingContext()
        {
            WasDisposed = false;
        }

        public void Dispose()
        {
            WasDisposed = true;
        }
    }

    [Context(Lifetime = ContextLifetime.Story)]
    public class TestStoryLifetimeMappingContext : IDisposable
    {
        public static bool WasDisposed;

        public TestStoryLifetimeMappingContext()
        {
            WasDisposed = false;
        }

        public void Dispose()
        {
            WasDisposed = true;
        }
    }
}