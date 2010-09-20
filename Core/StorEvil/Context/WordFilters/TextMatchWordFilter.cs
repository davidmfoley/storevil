using System.Collections.Generic;
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
        public IEnumerable<WordMatch> GetMatches(string[] s)
        {
            var lowerWord    = Word.ToLower();
            var lowerTarget = s[0].ToLower();

            var isMatch = lowerWord == lowerTarget.ToCSharpName() || lowerWord == lowerTarget;
            if (!isMatch)
                return WordMatch.NoMatch();

            return new[] { new WordMatch(1, s[0])};
        }
    }
}