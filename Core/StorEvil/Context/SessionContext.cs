using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StorEvil.Context
{
    public class SessionContext : ISessionContext
    {
        private readonly List<Type> _contextTypes = new List<Type>();
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();
        private List<Assembly> _assemblies = new List<Assembly>();

        public SessionContext(AssemblyRegistry assemblyRegistry)
        {
            foreach (var type in assemblyRegistry.GetTypesWithCustomAttribute<ContextAttribute>())
            {
                AddContext(type);
            }
        }

        public SessionContext()
        {
        }


        public void AddContext<T>() where T : class
        {
            AddContext(typeof(T));
        }

        private void AddContext(Type t)
        {
            _contextTypes.Add(t);
        }

        public void AddAssembly(Assembly a)
        {
            var allTypesInAssembly = a.GetTypes();
            var storEvilContexts = allTypesInAssembly
                .Where(TypeHasContextAttrbiute)
                .Where(NotAlreadyLoaded);

            _assemblies.Add(a);
            foreach (var t in storEvilContexts)
                AddContext(t);
        }

        private bool NotAlreadyLoaded(Type t)
        {
            return !_contextTypes.Contains(t);
        }

        private static bool TypeHasContextAttrbiute(Type t)
        {
            return t.GetCustomAttributes(true).Any(x => x.GetType().FullName == typeof(ContextAttribute).FullName);
        }

        public void AddAssembly(string pathToAssembly)
        {
            var a = Assembly.LoadFrom(pathToAssembly);
            AddAssembly(a);
        }

        public StoryContext GetContextForStory()
        {
            return new StoryContext(this, _contextTypes.Union(new[] { typeof(object) }), _cache);
        }

        public void SetContext(object context)
        {
            _cache.Add(context.GetType(), context);
        }

        public IEnumerable<Assembly> GetAllAssemblies()
        {
            return _contextTypes.Select(x => x.Assembly).Union(_assemblies).Distinct();
        }

        public void Dispose()
        {
            foreach (var context in _cache)
            {
                var disposable = context.Value as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }
}