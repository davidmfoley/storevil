using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Nunit;

namespace StorEvil.Context
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

        private void BuildMethodWordFilters()
        {
            var parameterInfos = GetMethodParameterInfos(_methodInfo);
            var paramNameMap = GetParameterNameToInfoMap(parameterInfos);

            foreach (var word in _nameSplitter.SplitMemberName(MemberInfo.Name))
                AddWordFilter(word, paramNameMap);

            AppendUnmatchedParameters(parameterInfos, paramNameMap);
        }

        private void AppendUnmatchedParameters(ParameterInfo[] parameterInfos,  Dictionary<string, ParameterInfo> paramNameMap)
        {
            // any additional parameters 
            // (that do not match pieces of the method name) 
            // are appended
            foreach (var parameter in parameterInfos)
                if (paramNameMap.ContainsKey(parameter.Name))
                    _wordFilters.Add(new ParameterMatchWordFilter(parameter));
        }

        private Dictionary<string, ParameterInfo> GetParameterNameToInfoMap(ParameterInfo[] parameterInfos)
        {
            var paramNameMap = new Dictionary<string, ParameterInfo>();
            foreach (var parameter in parameterInfos)
                paramNameMap.Add(parameter.Name, parameter);
            return paramNameMap;
        }

        private static ParameterInfo[] GetMethodParameterInfos(MethodInfo methodInfo)
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
            if (paramNameMap.ContainsKey(word))
            {
                _wordFilters.Add(new ParameterMatchWordFilter(paramNameMap[word]));
                paramNameMap.Remove(word);
            }
            else
            {
                _wordFilters.Add(new TextMatchWordFilter(word));
            }
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

            return BuildNameMatch(words, paramValues);
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