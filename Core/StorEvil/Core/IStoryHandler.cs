using System.Collections.Generic;

namespace StorEvil.Core
{
    public interface IStoryHandler
    {
        //void HandleStory(Story story);
        JobResult HandleStories(IEnumerable<Story> stories);       
    }
}