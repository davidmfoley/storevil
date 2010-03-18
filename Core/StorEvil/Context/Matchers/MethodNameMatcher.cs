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
    /// Matches the reflected name of an instance method or extension method on a context class
    /// to words in a scenario line.
    /// </summary>
    public class MethodNameMatcher : IMemberMatcher
    {
        private readonly MethodInfo _methodInfo;
        private readonly ContextMemberNameSplitter _nameSplitter = new ContextMemberNameSplitter();
        private readonly List<WordFilter> _wordFilters = new List<WordFilter>();
        private readonly ScenarioLineParser _scenarioLineParser = new ScenarioLineParser();

        public MethodNameMatcher(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            BuildMethodWordFilters();
        }

        public MemberInfo MemberInfo
        {
            get { return _methodInfo; }
        }

        public IEnumerable<NameMatch> GetMatches(string line)
        {
            return new[] {GetMatch(line)};
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

            var paramValues = new Dictionary<string, object>();

            // check each word for a match
            for (int i = 0; i < _wordFilters.Count; i++)
            {
                if (!_wordFilters[i].IsMatch(words[i]))
                    return null;

                // if this is a parameter, add to our hash so we can resolve the (string) value later
                var paramFilter = _wordFilters[i] as ParameterMatchWordFilter;

                if (paramFilter != null)
                    paramValues.Add(paramFilter.ParameterName, words[i]);
            }

            var match = BuildNameMatch(words, paramValues);
            if (match != null)
            {               
                DebugTrace.Trace("Method name match", "Method = " + _methodInfo.DeclaringType.Name + "." +_methodInfo.Name);
                if (paramValues.Count() > 0)
                {
                    var paramValueString = string.Join(",",
                                                       match.ParamValues.Select(kvp => kvp.Key + ":" + kvp.Value).
                                                           ToArray());

                    DebugTrace.Trace("Method name match", "Params= " + paramValueString);
                }
            }
            return match;
        }

        private void BuildMethodWordFilters()
        {
            var parameterInfos = GetMethodParameterInfos(_methodInfo);
            var paramNameMap = GetParameterNameToInfoMap(parameterInfos);

            foreach (var word in _nameSplitter.SplitMemberName(MemberInfo.Name))
                AddWordFilter(word, paramNameMap);

            AppendUnmatchedParameters(parameterInfos, paramNameMap);
        }

        private void AppendUnmatchedParameters(IEnumerable<ParameterInfo> parameterInfos,
                                               IDictionary<string, ParameterInfo> paramNameMap)
        {
            var unmatchedParameters = parameterInfos.Where(p => paramNameMap.ContainsKey(p.Name));
            var wordFilters = unmatchedParameters.Select(p => new ParameterMatchWordFilter(p)).Cast<WordFilter>();
            _wordFilters.AddRange(wordFilters);
        }

        private static Dictionary<string, ParameterInfo> GetParameterNameToInfoMap(
            IEnumerable<ParameterInfo> parameterInfos)
        {
            return parameterInfos.ToDictionary(parameter => parameter.Name);
        }

        private static IEnumerable<ParameterInfo> GetMethodParameterInfos(MethodInfo methodInfo)
        {
            var parameterInfos = methodInfo.GetParameters();

            // if static (extension), trim the first param...
            if (methodInfo.IsStatic)
                parameterInfos = parameterInfos.Skip(1).ToArray();

            return parameterInfos;
        }

        private void AddWordFilter(string word, Dictionary<string, ParameterInfo> paramNameMap)
        {
            // if a word in the name exactly matches a parameter to the method
            // then we will parse that value inline
            if (!paramNameMap.ContainsKey(word))
            {
                _wordFilters.Add(new TextMatchWordFilter(word));
                return;
            }

            _wordFilters.Add(new ParameterMatchWordFilter(paramNameMap[word]));
            paramNameMap.Remove(word);
        }

        private NameMatch BuildNameMatch(IEnumerable<string> words, Dictionary<string, object> paramValues)
        {
            var matchedText = string.Join(" ", words.Take(_wordFilters.Count()).ToArray());

            if (words.Count() == _wordFilters.Count)
                return new ExactMatch(paramValues, matchedText);

            var remainingText = string.Join(" ", words.Skip(_wordFilters.Count()).ToArray());

            if (CanBeAPartialMatch())
                return new PartialMatch(MemberInfo, paramValues, matchedText, remainingText);

            return null;
        }

        private bool CanBeAPartialMatch()
        {
            return ((MethodInfo) MemberInfo).ReturnType != null;
        }
    }
}