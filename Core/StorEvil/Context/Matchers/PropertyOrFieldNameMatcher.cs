using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matches;
using StorEvil.Context.WordFilters;
using StorEvil.Interpreter;
using StorEvil.Parsing;

namespace StorEvil.Context.Matchers
{
    /// <summary>
    /// Matches the reflected name of a Property or Field on a context class
    /// to words in a scenario line.
    /// </summary>
    public class PropertyOrFieldNameMatcher : IMemberMatcher
    {
        private readonly WordFilterFactory _wordFilterFactory = new WordFilterFactory();

        private readonly List<WordFilter> _wordFilters = new List<WordFilter>();

        public IEnumerable<WordFilter> WordFilters { get { return _wordFilters; } }
        public MemberInfo MemberInfo { get; set; }
        public Type ReturnType
        {
            get 
            { 
                if (MemberInfo is PropertyInfo) 
                    return ((PropertyInfo)MemberInfo).PropertyType;
                return ((FieldInfo)MemberInfo).FieldType;                
            }
        }

        public IEnumerable<NameMatch> GetMatches(string line)
        {
            return new[] { GetMatch(line) };  
        }

        private readonly ContextMemberNameSplitter _nameSplitter = new ContextMemberNameSplitter();
        private readonly ScenarioLineParser _scenarioLineParser = new ScenarioLineParser();

        public PropertyOrFieldNameMatcher(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
            BuildNonMethodWordFilters();
        }

        private void BuildNonMethodWordFilters()
        {
            _wordFilters.AddRange(
                _nameSplitter
                    .SplitMemberName(MemberInfo.Name)
                    .Select(word => _wordFilterFactory.GetTextFilter(word))                   
                );
        }

        /// <summary>
        /// Determine whether this member is a full or partial match 
        /// (or no match) for the set of words that is passed in 
        /// </summary>
        /// <param name="line">a set of words that is part of a scenario</param>
        /// <returns></returns>
        public NameMatch GetMatch(string line)
        {
            var words = _scenarioLineParser.ExtractWordsFromScenarioLine(line);

            // can't match if not enough words to fill us up
            if (words.Count < _wordFilters.Count)
                return null;

            var isMatch = WordFiltersMatch(words);
            if (!isMatch)
                return null;

            var match = BuildNameMatch(words);
            DebugTrace.Trace("Property match", "Member = " + MemberInfo.DeclaringType.Name + "." + MemberInfo.Name);
            return match;
        }

        private bool WordFiltersMatch(IList<string> words)
        {
            int wordIndex = 0;
            for (int filterIndex = 0; filterIndex < _wordFilters.Count; filterIndex++)
            {
                var match = (_wordFilters[filterIndex].GetMatches(words.Skip(wordIndex).ToArray())).FirstOrDefault();

                if (match == null || !match.IsMatch)
                    return false;

                wordIndex += match.WordCount;
            }
            return true;
        }

        readonly Dictionary<string, object> _noParams = new Dictionary<string, object>();

        private NameMatch BuildNameMatch(IEnumerable<string> words)
        {
            var matchedText = JoinWords(words.Take(_wordFilters.Count()));

            // check for exact match
            if (words.Count() == _wordFilters.Count)
                return new ExactMatch(_noParams, matchedText);

            var remainingText = JoinWords(words.Skip(_wordFilters.Count()));

            return new PartialMatch(MemberInfo, _noParams, matchedText, remainingText);
        }

        private static string JoinWords(IEnumerable<string> words)
        {
            return string.Join(" ", words.ToArray());
        }
    }

    public class WordMatch
    {
        public WordMatch(int wordCount, string value)
        {
            WordCount = wordCount;
            Value = value;
        }

        public bool IsMatch { get { return WordCount > 0; } }

        public int WordCount { get; set; }

        public string Value
        {
            get; private set;
        }

        public static IEnumerable<WordMatch> NoMatch()
        {
            return new WordMatch[0];
        }
    }
}