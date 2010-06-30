using System;

namespace StorEvil.Context
{
    public interface ISessionContext
    {
        StoryContext GetContextForStory();
        void SetContext(object context);
    }

    public class ConflictingLifetimeException : Exception
    {
        public ConflictingLifetimeException(string message) : base(message)
        {
            
        }
    }
}