namespace StorEvil.Context.WordFilters
{
    public interface WordFilter
    {
        bool IsMatch(string s);
    }
}