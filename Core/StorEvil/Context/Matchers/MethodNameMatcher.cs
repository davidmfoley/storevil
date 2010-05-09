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
    /// Matches the reflected name of an instance method or extension method on a context class
    /// to words in a scenario line.
    /// </summary>
    public class MethodNameMatcher : IMemberMatcher
    {
        private readonly WordFilterFactory _wordFilterFactory = new WordFilterFactory();

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
            var words = _scenarioLineParser.ExtractWordsFromScenarioLine(line);

            // can't match if not enough words to fill us up
            if (words.Count < _wordFilters.Count)
                return new NameMatch[0];

            var paramValues = new Dictionary<string, object>();

            return GetMatchesRecursive(words, words, _wordFilters, paramValues);
        }

        public IEnumerable<NameMatch> GetMatches_(string line)
        {
            var words = _scenarioLineParser.ExtractWordsFromScenarioLine(line);

            // can't match if not enough words to fill us up
            if (words.Count < _wordFilters.Count)
                return new NameMatch[0];

            var paramValues = new Dictionary<string, object>();

            var wordIndex = 0;
            List<NameMatch> nameMatches = new List<NameMatch>();

            var wordFilters = _wordFilters;

            // check each word for a match
            for (int i = 0; i < wordFilters.Count; i++)
            {
                var currentFilter = wordFilters[i];

                var wordMatches = currentFilter.GetMatches(words.Skip(wordIndex).ToArray());

                var wordMatch = wordMatches.FirstOrDefault();
                if (wordMatch == null || !wordMatch.IsMatch)
                    return new NameMatch[0];

                foreach (var m in wordMatches)
                {
                    
                }

                // if this is a parameter, add to our hash so we can resolve the (string) value later
                var paramFilter = currentFilter as ParameterMatchWordFilter;

                if (paramFilter != null)
                    paramValues.Add(paramFilter.ParameterName, wordMatch.Value);

                wordIndex += wordMatch.WordCount;
            }

            var match = BuildNameMatch(words, paramValues, wordFilters.Count());


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
            return new[] { match};
        }

        private IEnumerable<NameMatch> GetMatchesRecursive(IEnumerable<string> allWords, IEnumerable<string> words, IEnumerable<WordFilter> filters,  Dictionary<string, object> paramValues)
        {
            if (!filters.Any())
            {
                yield return BuildNameMatch(allWords, paramValues, allWords.Count() - words.Count());
                yield break;
            }

            if (!words.Any())
            {
                yield break;
            }

            foreach (var match in filters.First().GetMatches(words.ToArray()) ?? new WordMatch[0])
            {
                var paramFilter = filters.First() as ParameterMatchWordFilter;
                var innerParamValues = paramValues;
                if (paramFilter != null)
                {
                    innerParamValues = CopyAndAdd(paramValues, paramFilter.ParameterName, match.Value);
                }
                foreach (
                    var nameMatch in
                        GetMatchesRecursive(allWords, words.Skip(match.WordCount), filters.Skip(1), innerParamValues).ToArray())
                {
                    yield return nameMatch;
                }
            }
        }

        private Dictionary<string, object> CopyAndAdd(Dictionary<string, object> paramValues, string key, object value)
        {
            var copied = new Dictionary<string, object>(paramValues);
            copied.Add(key, value);
            return copied;
        }

        private void BuildMethodWordFilters()
        {
            var parameterInfos = GetMethodParameterInfos(_methodInfo);
            var paramNameMap = GetParameterNameToInfoMap(parameterInfos);

            foreach (var word in _nameSplitter.SplitMemberName(MemberInfo.Name))
                AddWordFilter(word, paramNameMap);

            AppendUnmatchedParameters(parameterInfos, paramNameMap);
        }

        private static Dictionary<string, ParameterInfo> GetParameterNameToInfoMap(IEnumerable<ParameterInfo> parameterInfos)
        {
            return parameterInfos.ToDictionary(parameter => parameter.Name);
        }

        private void AppendUnmatchedParameters(IEnumerable<ParameterInfo> parameterInfos,
                                               IDictionary<string, ParameterInfo> paramNameMap)
        {
            var unmatchedParameters = parameterInfos.Where(p => paramNameMap.ContainsKey(p.Name));
            var wordFilters = unmatchedParameters.Select(p => _wordFilterFactory.GetParameterFilter(p)).Cast<WordFilter>();
            _wordFilters.AddRange(wordFilters);
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
                _wordFilters.Add(_wordFilterFactory.GetTextFilter(word));
                return;
            }

            _wordFilters.Add(_wordFilterFactory.GetParameterFilter(paramNameMap[word]));
            paramNameMap.Remove(word);
        }

        private NameMatch BuildNameMatch(IEnumerable<string> words, Dictionary<string, object> paramValues, int wordCount)
        {
            var matchedText = string.Join(" ", words.Take(wordCount).ToArray());

            if (words.Count() == wordCount)
                return new ExactMatch(paramValues, matchedText);

            var remainingText = string.Join(" ", words.Skip(wordCount).ToArray());

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