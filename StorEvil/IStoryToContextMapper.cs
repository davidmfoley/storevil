using System;
using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil
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

    public class ScenarioContext
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
                    _cache.Add(type, Activator.CreateInstance(type));

                return _cache[type];
            }
            catch
            {
                return null;
            }
        }

        public void SetContext(object context)
        {
            _cache[context.GetType()] = context;
        }
    }
}