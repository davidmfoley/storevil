using System;
using System.Collections.Generic;
using System.Linq;
using Funq;
using StorEvil.Core;

namespace StorEvil.Context
{
    public interface IStoryContextFactory
    {
        StoryContext GetContextForStory(Story story);
    }

    public class StoryContext : IDisposable
    {
        public StoryContext(params Type[] types)
        {
            ImplementingTypes = types;
        }

        public StoryContext(IEnumerable<Type> types)
        {
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
            _cache.Add(o.GetType(), o);
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

    public class ScenarioContext : IDisposable
    {
        private readonly StoryContext _parentContext;

        public ScenarioContext(StoryContext parentContext, IEnumerable<Type> implementingTypes, Dictionary<Type, object> outerContexts)
        {
            _cache = new Dictionary<Type, object>(outerContexts);

            _parentContext = parentContext;

            ImplementingTypes = implementingTypes;
        }

        public IEnumerable<Type> ImplementingTypes { get; set; }

        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();      

        public object GetContext(Type type)
        {
            try
            {
                if (!_cache.ContainsKey(type))
                {
                    _cache.Add(type, CreateContextObject(type));

                    if (HasStoryLifetime(type))
                    {
                        _parentContext.SetContext(_cache[type]);
                    }
                }
                return _cache[type];
            }
            catch
            {
                return null;
            }
        }

        private bool HasStoryLifetime(Type type)
        {
            var contextAttr = (ContextAttribute) type.GetCustomAttributes(typeof (ContextAttribute), true).FirstOrDefault();
            if (contextAttr == null)
                return false;

            return contextAttr.Lifetime == ContextLifetime.Story;
        }

        private object CreateContextObject(Type type)
        {
            var constructor = type
                .GetConstructors()
                .OrderBy(x => x.GetParameters().Length)
                .Where(ctor => !ctor.IsStatic)
                .Last();

            var parameters = constructor.GetParameters().Select(x=>GetContext(x.ParameterType));

            return Activator.CreateInstance(type, parameters.ToArray(), null);
        }

        public IDictionary<Type, object> Contexts
        {
            get { return _cache; }
        }

        public void SetContext(object context)
        {
            _cache[context.GetType()] = context;
        }

        public void Dispose()
        {
            foreach (var context in _cache.Values)
            {
                if (context is IDisposable && !HasStoryLifetime(context.GetType()))
                    ((IDisposable)context).Dispose();
            }
        }
    }
}