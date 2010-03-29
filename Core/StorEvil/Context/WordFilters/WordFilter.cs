using StorEvil.Context.Matchers;

namespace StorEvil.Context.WordFilters
{
    public interface WordFilter
    {
        WordMatch GetMatch(string[] s);
       
    }
}