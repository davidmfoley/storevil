using StorEvil.Context;

namespace StorEvil.Core
{
    public interface IStoryHandler
    {
        void HandleStory(Story story, StoryContext context);
        void Finished();
    }
}