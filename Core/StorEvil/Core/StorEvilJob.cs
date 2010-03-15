using StorEvil.Context;
using StorEvil.InPlace;

namespace StorEvil.Core
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

        public int Run()
        {
            int failed = 0;
            foreach (var story in StoryProvider.GetStories())
            {
                var context = StoryToContextMapper.GetContextForStory(story);

                failed += Handler.HandleStory(story, context);
            }

            Handler.Finished();
            return failed;
        }
    }

    public interface IStorEvilJob
    {
        int Run();
    }
}