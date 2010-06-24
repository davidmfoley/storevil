using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.Context
{
    public class SessionContext : ISessionContext, IDisposable
    {
        private readonly List<Type> _contextTypes = new List<Type>();
        private readonly ExtensionMethodHandler _extensionMethodHandler = new ExtensionMethodHandler();
        private readonly Dictionary<Type, object> _cache = new Dictionary<Type, object>();      

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
            var storEvilContexts = allTypesInAssembly.Where(TypeHasContextAttrbiute);

            foreach (var t in storEvilContexts)            
                AddContext(t);
        }
      
        private static bool TypeHasContextAttrbiute(Type t)
        {
            return t.GetCustomAttributes(typeof (ContextAttribute), true).Any();
        }

        public void AddAssembly(string pathToAssembly)
        {
            var a = Assembly.LoadFrom(pathToAssembly);
            AddAssembly(a);
            _extensionMethodHandler.AddAssembly(a);
        }

        public StoryContext GetContextForStory()
        {
            return new StoryContext(this, _contextTypes.Union(new[] {typeof (object)}), _cache);
        }

        public void SetContext(object context)
        {
            _cache.Add(context.GetType(), context);
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