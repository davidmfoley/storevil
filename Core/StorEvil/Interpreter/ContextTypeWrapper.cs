using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;

namespace StorEvil.Interpreter
{
    public class ContextTypeWrapper
    {
        private readonly Type _type;
        public List<IMemberMatcher> MemberMatchers = new List<IMemberMatcher>();
      
        public ContextTypeWrapper(Type type, IEnumerable<MethodInfo> extensionMethodsForType)
        {
            _type = type;         

            DebugTrace.Trace(GetType().Name, "Building wrapper for: " + type);

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (MemberInfo member in GetMembers(flags)) 
                AddMatchers(member);

            foreach (var methodInfo in extensionMethodsForType)
            {
                DebugTrace.Trace(GetType().Name, "Added extension method matcher: " + methodInfo.Name);

                MemberMatchers.Add(new MethodNameMatcher(methodInfo));
            }
        }

        public Type WrappedType { get { return _type; } }

        private void AddMatchers(MemberInfo member)
        {
            MemberMatchers.Add(GetMemberMatcher(member));

            DebugTrace.Trace(GetType().Name, "Added reflection matcher: " + member.Name);

            AddRegexMatchersIfAttributePresent(member);
        }

        private void AddRegexMatchersIfAttributePresent(MemberInfo member)
        {
            var regexAttrs = member.GetCustomAttributes(typeof(ContextRegexAttribute), true);
            foreach (var regexAttr in regexAttrs.Cast<ContextRegexAttribute>())
            {
                DebugTrace.Trace(GetType().Name, "Added regex matcher: " + member.Name + ", \"" + regexAttr.Pattern + "\"");

                MemberMatchers.Add(new RegexMatcher(regexAttr.Pattern, member));
            }
        }

        private static IMemberMatcher GetMemberMatcher(MemberInfo member)
        {
            if (member is MethodInfo)
                return new MethodNameMatcher((MethodInfo)member);

            return new PropertyOrFieldNameMatcher(member);
        }

        private MemberInfo[] GetMembers(BindingFlags flags)
        {
            var members = _type.GetMembers(flags);
           

            return FilterMembers(members);
        }

        private MemberInfo[] FilterMembers(MemberInfo[] members)
        {
            var ignore = new[] {"GetType", "ToString", "CompareTo", "GetTypeCode", "Equals", "GetHashCode"};
            return members.Where(m => !(m.MemberType == MemberTypes.Constructor || m.MemberType == MemberTypes.NestedType || ignore.Contains(m.Name))).ToArray();
        }
    }
}