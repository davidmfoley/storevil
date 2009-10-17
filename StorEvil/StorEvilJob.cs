using System;

namespace StorEvil
{
    /// <summary>
    /// Runs the process of mapping stories, building code, etc.
    /// </summary>
    public class StorEvilJob : IStorEvilJob
    {
        public IStoryProvider StoryProvider { get; set; }
        public IStoryToContextMapper StoryToContextMapper { get; set; }
        public IStoryHandler Handler { get; set; }
     
        public StorEvilJob(
            IStoryProvider storyProvider, 
            IStoryToContextMapper storyToContextMapper,
            IStoryHandler handler)
            
        {
            StoryProvider = storyProvider;
            StoryToContextMapper = storyToContextMapper;
            Handler = handler;

        }

        public void Run()
        {
            foreach(var story in StoryProvider.GetStories())
            {
                var context = StoryToContextMapper.GetContextForStory(story);

                Handler.HandleStory(story, context);
            }

            Handler.Finished();
        }
    }

    public interface IStorEvilJob
    {
        void Run();
    }
}