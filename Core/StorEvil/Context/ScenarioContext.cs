using System;
using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Context
{
    public class ScenarioContext : IDisposable
    {
        private readonly StoryContext _parentContext;

        public ScenarioContext(StoryContext parentContext, IEnumerable<Type> implementingTypes, IDictionary<Type, object> outerContexts)
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
                EnsureContextObjectExists(type);
                return _cache[type];
            }
            catch (ConflictingLifetimeException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }

        private void EnsureContextObjectExists(Type type)
        {
            if (_cache.ContainsKey(type)) 
                return;

            _cache.Add(type, CreateContextObject(type));

            if (HasStoryOrSessionLifetime(type))
                _parentContext.SetContext(_cache[type]);
        }

        private bool HasStoryOrSessionLifetime(Type type)
        {            
            var contextLifetime = GetLifetime(type);

            return contextLifetime == ContextLifetime.Story || contextLifetime == ContextLifetime.Session;
        }

        private ContextLifetime GetLifetime(Type type)
        {
            var contextAttr = (ContextAttribute)type.GetCustomAttributes(typeof(ContextAttribute), true).FirstOrDefault();
            return contextAttr == null ? ContextLifetime.Scenario : contextAttr.Lifetime;
        }

        private object CreateContextObject(Type type)
        {
            var constructor = type
                .GetConstructors()
                .OrderBy(x => x.GetParameters().Length)
                .Where(ctor => !ctor.IsStatic)
                .Last();

            var parameters = constructor.GetParameters().Select(x=>GetContext(x.ParameterType));

            DetectConflictingLifetimes(type, parameters.Select(x => x.GetType()));

            return Activator.CreateInstance(type, parameters.ToArray(), null);
        }

        private void DetectConflictingLifetimes(Type dependentType, IEnumerable<Type> dependedOnTypes)
        {
            ContextLifetime dependentLifetime = GetLifetime(dependentType);
           
            var errors = dependedOnTypes.Where(x => GetLifetime(x) < dependentLifetime);
            if (!errors.Any())
                return;

            var messages = errors.Select(x => dependentType.FullName + " => " + x.FullName).ToArray();
            var message = "The following dependencies could not be resolved because a context type cannot depend on a type that has a shorter lifetime:\r\n"
                          + string.Join("\r\n", messages);

            throw new ConflictingLifetimeException(message);
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
                if (context is IDisposable && !HasStoryOrSessionLifetime(context.GetType()))
                    ((IDisposable)context).Dispose();
            }
        }
    }
}