using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using StorEvil.Context.Matches;

namespace StorEvil.Context.Matchers
{
    public class RegexMatcher : IMemberMatcher
    {
        private readonly Regex _regex;

        public RegexMatcher(string pattern, MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;

            _regex = new Regex(pattern);
        }

        public MemberInfo MemberInfo
        {
            get;
            private set;
        }

        public IEnumerable<NameMatch> GetMatches(string line)
        {
            return new[] { GetMatch(line) };  
        }

        public NameMatch GetMatch(string line)
        {
            var result = _regex.Match(line);
            if (result.Success)
            {
                if (result.Length == line.Length)
                    return new ExactMatch(GetParameters(result), line);
                
                if (PartialMatchSupportedByMember())
                    return new PartialMatch(MemberInfo, GetParameters(result), result.Value,
                                            line.Substring(result.Length).Trim());
            }
            return null;
        }

        private Dictionary<string, object> GetParameters(Match result)
        {
            var parameters = new Dictionary<string, object>();
            var methodInfo = MemberInfo as MethodInfo;
            if (methodInfo == null)
                return parameters;

            int i = 1;
            foreach (var parameter in methodInfo.GetParameters())
                parameters.Add(parameter.Name, result.Groups[i++]);

            return parameters;
        }

        private bool PartialMatchSupportedByMember()
        {
            return !(MemberInfo is MethodInfo) || ((MethodInfo)MemberInfo).ReturnType != typeof(void);
        }

     
    }
}