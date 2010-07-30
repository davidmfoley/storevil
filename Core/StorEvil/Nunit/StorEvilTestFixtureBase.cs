using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.NUnit
{
    public class StorEvilTestFixtureBase
    {
        private Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        protected void SetUp()
        {
            
        }

        protected void CleanUpScenario()
        {
            var typesToCleanup = _cache.Keys.Where(IsScenarioLifetime) ?? new Type[0];

            DestroyContexts(typesToCleanup);
        }

        protected void CleanUpStory()
        {
            var typesToCleanup = _cache.Keys.Where(IsStoryLifetime);

            DestroyContexts(typesToCleanup);
        }

        private void DestroyContexts(IEnumerable<Type> typesToCleanup)
        {
            foreach (var type in typesToCleanup.ToArray())
            {
                var context = _cache[type];
                if (context is IDisposable)
                    ((IDisposable)context).Dispose();

                _cache.Remove(type);
            }
        }

        private bool IsScenarioLifetime(Type type)
        {
            return GetLifetime(type) == ContextLifetime.Scenario;
        }

        private ContextLifetime GetLifetime(Type type)
        {
            var attribute = (StorEvil.ContextAttribute)type.GetCustomAttributes(typeof (StorEvil.ContextAttribute), true).FirstOrDefault();
            if (attribute == null)
                return ContextLifetime.Scenario;

            return attribute.Lifetime;
        }

        private bool IsStoryLifetime(Type type)
        {
            return GetLifetime(type) != ContextLifetime.Scenario;
        }

        protected T GetContext<T>()
        {
            var type = typeof (T);
            if (!_cache.ContainsKey(type))
                _cache.Add(type, Activator.CreateInstance(type));

            return (T)_cache[type];
        }
        
    }
}