using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using StorEvil.Nunit;

namespace StorEvil.Context
{
    /// <summary>
    /// A member (method, property or field) on a test context class 
    /// 
    /// Test context class implements the behavior that stories and scenarios invoke.
    /// </summary>
    /// <remarks>
    /// might be wise to break out Methods from Fields and Properties here, since Methods have parameters.
    /// </remarks>
    public class MemberNameMatcher : IMemberMatcher
    {
        private readonly List<WordFilter> _wordFilters = new List<WordFilter>();
        public MemberInfo MemberInfo { get; set; }

        public MemberNameMatcher(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;

            var paramNameMap = new Dictionary<string, ParameterInfo>();
            var parameters = new ParameterInfo[] { };

            // if we are dealing with a method, get the parameters
            if (MemberInfo is MethodInfo)
            {
                // if static (extension), trim the first param...
                var methodInfo = (MethodInfo)MemberInfo;
                parameters = methodInfo.GetParameters();

                if (methodInfo.IsStatic)
                {
                    parameters = parameters.Skip(1).ToArray();
                }

                foreach (var parameter in parameters)
                    paramNameMap.Add(parameter.Name, parameter);
            }

            // split into pieces 
            foreach (var word in SplitMemberName(MemberInfo.Name))
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

            // any additional parameters 
            // (that do not match pieces of the method name) 
            // are appended
            foreach (var parameter in parameters)
                if (paramNameMap.ContainsKey(parameter.Name))
                    _wordFilters.Add(new ParameterMatchWordFilter(parameter));
        }

        static readonly Regex SplitMemberNameRegex = new Regex("[A-Z]?[a-z]*");
        private ScenarioLineParser _scenarioLineParser = new ScenarioLineParser();

        public static IEnumerable<string> SplitMemberName(string name)
        {
            if (name.Contains("_"))
            {
                foreach (var s in name.Split('_'))
                    yield return s;
            }
            else
            {
                foreach (Match m in SplitMemberNameRegex.Matches(name))
                    if (!string.IsNullOrEmpty(m.Value))
                        yield return m.Value;
            }
        }


        /// <summary>
        /// Determine whether this member is a full or partial match 
        /// (or no match) for the set of words that is passed in 
        /// </summary>
        /// <param name="words">a set of words that is part of a scenario</param>
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

        /// <summary>
        /// </summary>
        /// <param name="words"></param>
        /// <param name="paramValues"></param>
        /// <returns>
        /// note: Partial Match means part of the line was matched
        /// 
        /// </returns>
        private NameMatch BuildNameMatch(IEnumerable<string> words, Dictionary<string, object> paramValues)
        {
            var matchedText = string.Join(" ", words.Take(_wordFilters.Count()).ToArray());
            var remainingText = string.Join(" ", words.Skip(_wordFilters.Count()).ToArray());
            // check for exact match
            if (words.Count() == _wordFilters.Count)
                return new ExactMatch(paramValues, matchedText);

            if (MemberInfo is FieldInfo)
                return new PartialMatch(MemberInfo, paramValues, matchedText, remainingText);

            if (MemberInfo is PropertyInfo)
                return new PartialMatch(MemberInfo, paramValues, matchedText, remainingText);

            // note: void methods cannot be partial matches (must return a value)
            if (MemberInfo is MethodInfo && ((MethodInfo)MemberInfo).ReturnType != null)
                return new PartialMatch(MemberInfo, paramValues, matchedText, remainingText);
            return null;
        }
    }
}