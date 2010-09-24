using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;
using StorEvil.Utility;

namespace StorEvil.Interpreter
{
    public class RegExMatcherFactory : IMatcherFactory
    {
        private MemberReader _memberReader = new MemberReader();

        public IEnumerable<IMemberMatcher> GetMatchers(Type type)
        {
            foreach (MemberInfo member in _memberReader.GetMembers(type, BindingFlags.Instance | BindingFlags.Public))
            {
                var regexAttrs = member.GetCustomAttributes( true).Where(x=>x.GetType().Name == typeof(ContextRegexAttribute).Name);

                foreach (var regexAttr in regexAttrs)
                {
                    var pattern = (string)regexAttr.ReflectionGet("Pattern");

                    DebugTrace.Trace(GetType().Name,
                                     "Added regex matcher: " + member.Name + ", \"" + pattern + "\"");

                    yield return new RegexMatcher(pattern, member);
                }
            }
        }
    }
}