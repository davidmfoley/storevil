using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;

namespace StorEvil.Interpreter
{
    public class ReflectionMatcherFactory : IMatcherFactory
    {
        private MemberReader _memberReader = new MemberReader();
        public IEnumerable<IMemberMatcher> GetMatchers(Type type)
        {
            foreach (MemberInfo member in GetMembersWeCareAbout(type))
            {
                var reflectionMatcher = GetMemberMatcher(member);

                if (reflectionMatcher != null)
                {
                    DebugTrace.Trace(GetType().Name, "Added reflection matcher: " + member.Name);

                    yield return reflectionMatcher;
                }
            }
        }

        private IEnumerable<MemberInfo> GetMembersWeCareAbout(Type type)
        {
            var members =  _memberReader.GetMembers(type,  BindingFlags.Instance | BindingFlags.Public);
            return members.Where(m => IsNotGarbage(type, m));
        }

        private static bool IsNotGarbage(Type t, MemberInfo m)
        {
            if (m is MethodInfo && (m.Name.StartsWith("get_") || m.Name.StartsWith("set_")) && m.Name.Length > 4)
                return t.GetProperty(m.Name.Substring(4)) == null;

            return true;
        }

        private static IMemberMatcher GetMemberMatcher(MemberInfo member)
        {
            if (member is MethodInfo) 
                return new MethodNameMatcher((MethodInfo)member);

            return new PropertyOrFieldNameMatcher(member);
        }
    }
}