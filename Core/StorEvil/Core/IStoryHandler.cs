using StorEvil.Context;

namespace StorEvil.Core
{
    public interface IStoryHandler
    {
        void HandleStory(Story story);
        void Finished();
        StorEvilResult GetResult();
    }
}