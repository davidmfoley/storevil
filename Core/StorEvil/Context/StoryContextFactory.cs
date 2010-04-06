using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using StorEvil.Core;
using StorEvil.Interpreter;

namespace StorEvil.Context
{
    public class StoryContextFactory : IStoryContextFactory
    {
        private readonly List<Type> _contextTypes = new List<Type>();
        private ExtensionMethodHandler _extensionMethodHandler = new ExtensionMethodHandler();

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

        public StoryContext GetContextForStory(Story story)
        {
            //if (_contextTypes.Count() == 0)
            //    throw new ConfigurationErrorsException("no context types have been registered");

            return new StoryContext(_contextTypes.Union(new[] {typeof (object)}));
        }
    }
}