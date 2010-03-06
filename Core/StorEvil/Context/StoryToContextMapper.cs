using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using StorEvil.Core;

namespace StorEvil.Context
{
    public class StoryToContextMapper : IStoryToContextMapper
    {
        private readonly List<Type> _contextTypes = new List<Type>();

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
            foreach (var t in a.GetTypes())
            {
                if (t.GetCustomAttributes(typeof (ContextAttribute), true).Any())
                    AddContext(t);
            }
        }

        public void AddAssembly(string pathToAssembly)
        {
            var a = Assembly.LoadFrom(pathToAssembly);
            AddAssembly(a);
        }

        public StoryContext GetContextForStory(Story story)
        {
            if (_contextTypes.Count() == 0)
                throw new ConfigurationErrorsException("no context types have been registered");

            return new StoryContext(_contextTypes.Union(new[] {typeof (object)}));
        }
    }
}