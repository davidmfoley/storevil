using System.Collections.Generic;
using StorEvil.Context.Matchers;

namespace StorEvil.Context.WordFilters
{
    public interface WordFilter
    {
        IEnumerable<WordMatch> GetMatches(string[] s);
       
    }
}