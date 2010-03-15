using StorEvil.Context;

namespace StorEvil.Core
{
    public interface IStoryHandler
    {
        int HandleStory(Story story, StoryContext context);
        void Finished();
    }
}