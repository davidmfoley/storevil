using StorEvil.Core;

namespace StorEvil
{
    public interface IStoryHandler
    {
        void HandleStory(Story story, StoryContext context);
        void Finished();
    }
}