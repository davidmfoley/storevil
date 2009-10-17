using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil
{
    public interface IStoryProvider
    {
        IEnumerable<Story> GetStories();
    }
}
