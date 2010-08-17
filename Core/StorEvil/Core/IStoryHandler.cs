using System.Collections.Generic;

namespace StorEvil.Core
{
    public interface IStoryHandler
    {
        //void HandleStory(Story story);
        void HandleStories(IEnumerable<Story> stories);
        void Finished();
        JobResult GetResult();
    }
}