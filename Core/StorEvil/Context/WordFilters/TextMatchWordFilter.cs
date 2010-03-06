namespace StorEvil.Context.WordFilters
{
    public class TextMatchWordFilter : WordFilter
    {
        public TextMatchWordFilter(string word)
        {
            Word = word;
        }

        public string Word { get; set; }
        public bool IsMatch(string s)
        {
            return Word.ToLower() == s.ToLower();
        }
    }
}