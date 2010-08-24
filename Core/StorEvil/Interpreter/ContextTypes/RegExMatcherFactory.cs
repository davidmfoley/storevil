using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;

namespace StorEvil.Interpreter
{
    public class RegExMatcherFactory : IMatcherFactory
    {
        private MemberReader _memberReader = new MemberReader();

        public IEnumerable<IMemberMatcher> GetMatchers(Type type)
        {
            foreach (MemberInfo member in _memberReader.GetMembers(type, BindingFlags.Instance | BindingFlags.Public))
            {
                var regexAttrs = member.GetCustomAttributes(typeof(ContextRegexAttribute), true);
                foreach (var regexAttr in regexAttrs.Cast<ContextRegexAttribute>())
                {
                    DebugTrace.Trace(GetType().Name,
                                     "Added regex matcher: " + member.Name + ", \"" + regexAttr.Pattern + "\"");

                    yield return new RegexMatcher(regexAttr.Pattern, member);
                }
            }
        }
    }
}