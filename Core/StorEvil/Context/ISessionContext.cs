using System;
using System.Collections.Generic;
using System.Reflection;

namespace StorEvil.Context
{
    public interface ISessionContext : IDisposable
    {
        StoryContext GetContextForStory();
        void SetContext(object context);
        IEnumerable<Assembly> GetAllAssemblies();
    }

    public class ConflictingLifetimeException : Exception
    {
        public ConflictingLifetimeException(string message) : base(message)
        {
            
        }
    }
}