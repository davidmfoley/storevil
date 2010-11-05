using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using StorEvil.Context.Matches;
using StorEvil.Interpreter;

namespace StorEvil.Context.Matchers
{
    public class RegexMatcher : IMemberMatcher
    {
        private readonly Regex _regex;

        public RegexMatcher(string pattern, MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;

            _regex = new Regex(pattern);
            Pattern = pattern;
        }

        public MemberInfo MemberInfo
        {
            get;
            private set;
        }

        public string Pattern { get; private set; }
           

        public IEnumerable<NameMatch> GetMatches(string line)
        {
            return new[] { GetMatch(line) };  
        }

        public Type ReturnType
        {
            get
            {
                if (MemberInfo is PropertyInfo)
                    return ((PropertyInfo)MemberInfo).PropertyType;
                if (MemberInfo is FieldInfo)
                    return ((FieldInfo)MemberInfo).FieldType;
                return ((MethodInfo) MemberInfo).ReturnType;
            }
        }

        public NameMatch GetMatch(string line)
        {
            var result = _regex.Match(line);
            NameMatch match = null;
            if (result.Success)
            {
                if (result.Length == line.Length)
                    match = new ExactMatch(GetParameters(result), line);                
                else if (PartialMatchSupportedByMember())
                    match = new PartialMatch(MemberInfo, GetParameters(result), result.Value,
                                            line.Substring(result.Length).Trim());
            }
            if (match != null)
            {
                DebugTrace.Trace("RegExp match",
                                 "Member = " + MemberInfo.DeclaringType.Name + "." + MemberInfo.Name);
            }
            return match;
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