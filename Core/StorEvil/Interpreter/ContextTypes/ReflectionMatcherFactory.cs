using System;
using System.Collections.Generic;
using System.Reflection;
using StorEvil.Context.Matchers;

namespace StorEvil.Interpreter
{
    public class ReflectionMatcherFactory : IMatcherFactory
    {
        private MemberReader _memberReader = new MemberReader();
        public IEnumerable<IMemberMatcher> GetMatchers(Type type)
        {
            foreach (MemberInfo member in _memberReader.GetMembers(type,  BindingFlags.Instance | BindingFlags.Public))
            {
                var reflectionMatcher = GetMemberMatcher(member);

                if (reflectionMatcher != null)
                {
                    DebugTrace.Trace(GetType().Name, "Added reflection matcher: " + member.Name);

                    yield return reflectionMatcher;
                }
            }
        }

        private static IMemberMatcher GetMemberMatcher(MemberInfo member)
        {
            if (member is MethodInfo)
                return new MethodNameMatcher((MethodInfo)member);

            return new PropertyOrFieldNameMatcher(member);
        }
    }
}