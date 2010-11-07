using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StorEvil.Context
{
    public class ScenarioContext : IDisposable
    {
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();
        private readonly StoryContext _parentContext;
        private bool _isDisposed;

        public ScenarioContext(StoryContext parentContext, IEnumerable<Type> implementingTypes,
                               IDictionary<Type, object> outerContexts)
        {
            _cache = new Dictionary<Type, object>(outerContexts);
            _parentContext = parentContext;
            ImplementingTypes = implementingTypes;
        }

        public IEnumerable<Type> ImplementingTypes { get; set; }

        public IDictionary<Type, object> Contexts
        {
            get { return _cache; }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                try
                {
                    foreach (IDisposable context in _cache.Values.Where(ShouldBeDisposedAtScenarioLevel))
                        context.Dispose();
                }
                finally
                {
                    _isDisposed = true;
                }
            }
        }

        private bool ShouldBeDisposedAtScenarioLevel(object context)
        {
            return context is IDisposable &&  context.GetType().GetContextInfo().Lifetime == ContextLifetime.Scenario;
        }

        public object GetContext(Type type)
        {
            EnsureContextObjectExists(type);
            return _cache[type];
        }

        private void EnsureContextObjectExists(Type type)
        {
            if (_cache.ContainsKey(type))
                return;

            _cache.Add(type, CreateContextObject(type));

            if (type.GetContextInfo().Lifetime != ContextLifetime.Scenario)
                _parentContext.SetContext(_cache[type]);
        }

        private object CreateContextObject(Type type)
        {
            ConstructorInfo constructor = type
                .GetConstructors()
                .OrderBy(x => x.GetParameters().Length)
                .Where(ctor => !ctor.IsStatic)
                .Last();

            IEnumerable<object> parameters = constructor.GetParameters().Select(x => GetContext(x.ParameterType));

            DetectConflictingLifetimes(type, parameters.Select(x => x.GetType()));

            return Activator.CreateInstance(type, parameters.ToArray(), null);
        }

        private void DetectConflictingLifetimes(Type dependentType, IEnumerable<Type> dependedOnTypes)
        {
            ContextLifetime dependentLifetime = dependentType.GetContextInfo().Lifetime;

            IEnumerable<Type> errors = dependedOnTypes.Where(x => x.GetContextInfo().Lifetime < dependentLifetime);
            if (!errors.Any())
                return;

            string[] messages = errors.Select(x => dependentType.FullName + " => " + x.FullName).ToArray();
            string message = "The following dependencies could not be resolved because a context type cannot depend on a type that has a shorter lifetime:\r\n"
                             + string.Join("\r\n", messages);

            throw new ConflictingLifetimeException(message);
        }

        public void SetContext(object context)
        {
            _cache[context.GetType()] = context;
        }
    }
}