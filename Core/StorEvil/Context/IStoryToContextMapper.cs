using System;
using System.Collections.Generic;
using System.Linq;
using Funq;
using StorEvil.Core;

namespace StorEvil.Context
{
    public interface IStoryToContextMapper
    {
        StoryContext GetContextForStory(Story story);
    }

    public class StoryContext
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

        public ScenarioContext GetScenarioContext()
        {
            return new ScenarioContext(ImplementingTypes);
        }
    }

    public class ScenarioContext : IDisposable
    {
        public ScenarioContext(IEnumerable<Type> implementingTypes)
        {
            
            ImplementingTypes = implementingTypes;
        }

        public IEnumerable<Type> ImplementingTypes { get; set; }

        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public object GetContext(Type type)
        {
            try
            {
                if (!_cache.ContainsKey(type))
                    _cache.Add(type, CreateContextObject(type));

                return _cache[type];
            }
            catch
            {
                return null;
            }
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

        public void SetContext(object context)
        {
            _cache[context.GetType()] = context;
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