using StorEvil.Context.Matchers;
using StorEvil.Utility;

namespace StorEvil.Context.WordFilters
{
    public class TextMatchWordFilter : WordFilter
    {
        public TextMatchWordFilter(string word)
        {
            Word = word;
        }

        public string Word { get; set; }
        public WordMatch GetMatch(string[] s)
        {
            var isMatch = Word.ToLower() == s[0].ToLower().ToCSharpName();
            if (!isMatch)
                return WordMatch.NoMatch();

            return new WordMatch(1, s[0]);
        }
    }
}