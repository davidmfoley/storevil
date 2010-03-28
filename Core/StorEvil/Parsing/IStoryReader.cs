using System.Collections.Generic;

namespace StorEvil.Parsing
{
    public interface IStoryReader
    {
        IEnumerable<StoryInfo> GetStoryInfos();
    }
}