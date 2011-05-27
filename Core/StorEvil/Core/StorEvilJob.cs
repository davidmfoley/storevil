using System;
using StorEvil.Context;

namespace StorEvil.Core
{
    /// <summary>
    /// Runs the process of mapping stories, building code, etc.
    /// </summary>
    public class StorEvilJob : IStorEvilJob
    {
        public IStoryProvider StoryProvider { get; set; }
        public IStoryHandler Handler { get; set; }
        public ISessionContext SessionContext { get; set; }

        public StorEvilJob(IStoryProvider storyProvider, IStoryHandler handler, ISessionContext sessionContext)
        {
            StoryProvider = storyProvider;
            Handler = handler;
            SessionContext = sessionContext;
        }

        public int Run()
        {       
            var failed =  Handler.HandleStories(StoryProvider.GetStories()).Failed;
            // hack
            SessionContext.Dispose();

            return failed;
        }
    }

    public interface IStorEvilJob
    {
        int Run();
    }
}