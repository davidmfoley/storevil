using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Configuration;
using StorEvil.Context;
using StorEvil.Interpreter;

namespace StorEvil.Context
{   

    public class AssemblyRegistry
    {
        private IEnumerable<Type> _allTypes;

         public AssemblyRegistry()
         {
             _allTypes = new Type[0];
         }

        public AssemblyRegistry(IEnumerable<Assembly> assemblies)
        {
            _allTypes = assemblies.SelectMany(a => a.GetTypes());
        }
        public AssemblyRegistry(IEnumerable<string> assemblyLocations)
        {
            _allTypes = assemblyLocations.Select(Assembly.LoadFrom).SelectMany(a => a.GetTypes());
        }

        public IEnumerable<Type> GetTypesWithCustomAttribute<T>()
        {
            return _allTypes.Where(t => TypeHasCustomAttribute(t, typeof (T)));               
        }
    
        private static bool TypeHasCustomAttribute(Type t, Type customAttribute)
        {
            // tolerate version differences between runner and target of context assembly
            return t.GetCustomAttributes(true).Any(x => x.GetType().FullName == customAttribute.FullName);
        }

        public IEnumerable<Type> GetTypesImplementing<T>()
        {
            Type targetType = typeof(T);
            return _allTypes.Where(t =>  t.IsSubclassOf(targetType) || t.GetInterfaces().Contains(targetType));
        }

        public IEnumerable<Type> GetStaticClasses()
        {
            return _allTypes.Where(IsStatic);

        }

        private bool IsStatic(Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }
    }

    public class SessionContext : ISessionContext, IDisposable
    {
        private readonly List<Type> _contextTypes = new List<Type>();
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();
        private List<Assembly> _assemblies = new List<Assembly>() ;

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
            AddContext(typeof (T));
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

        private  bool NotAlreadyLoaded(Type t)
        {
            return !_contextTypes.Contains(t);
        }

        private static bool TypeHasContextAttrbiute(Type t)
        {
            return t.GetCustomAttributes(true).Any(x=>x.GetType().FullName ==  typeof(ContextAttribute).FullName);
        }

        public void AddAssembly(string pathToAssembly)
        {
            var a = Assembly.LoadFrom(pathToAssembly);
            AddAssembly(a);            
        }

        public StoryContext GetContextForStory()
        {
            return new StoryContext(this, _contextTypes.Union(new[] {typeof (object)}), _cache);
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
                if (disposable!= null)
                    disposable.Dispose();
            }
        }
    }
}