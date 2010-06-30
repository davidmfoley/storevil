using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Context
{
    public class StoryContext : IDisposable
    {
        private readonly ISessionContext _parent;

        public StoryContext(ISessionContext parent, params Type[] types)
        {
            _parent = parent;
            ImplementingTypes = types;
        }

        public StoryContext(ISessionContext parent, IEnumerable<Type> types, Dictionary<Type, object> cache)
        {
            _parent = parent;
            _cache = cache;
            _cache = new Dictionary<Type, object>(cache);
            ImplementingTypes = types;
        }

        public IEnumerable<Type> ImplementingTypes { get; set; }
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();      

        public ScenarioContext GetScenarioContext()
        {
            return new ScenarioContext(this, ImplementingTypes, _cache);
        }

        public void SetContext(object o)
        {
            var type = o.GetType();
            var contextAttribute = (ContextAttribute) type.GetCustomAttributes(typeof(ContextAttribute), true).FirstOrDefault();
            if (contextAttribute != null && contextAttribute.Lifetime == ContextLifetime.Session)
                _parent.SetContext(o);
           
            _cache.Add(type, o);
        }

        public void Dispose()
        {
            foreach (var context in _cache.Values)
            {
                if (context is IDisposable)
                    ((IDisposable)context).Dispose();
            }
        }
    }
}