using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil.InPlace
{
    public interface IStoryProvider
    {
        IEnumerable<Story> GetStories();
    }
}


