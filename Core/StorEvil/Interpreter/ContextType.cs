using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;

namespace StorEvil.Interpreter
{
    public class ContextTypeFactory
    {
        private readonly IExtensionMethodHandler _extensionMethodHandler;

        public ContextTypeFactory(IExtensionMethodHandler extensionMethodHandler)
        {
            _extensionMethodHandler = extensionMethodHandler;
        }

        public ContextType GetWrapper(Type type)
        {
            var matchers = new List<IMemberMatcher>();

            DebugTrace.Trace(GetType().Name, "Building wrapper for: " + type);

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (MemberInfo member in GetMembers(type, flags))
            {
                matchers.AddRange(GetMatchers(member));               
            }
            foreach (var methodInfo in _extensionMethodHandler.GetExtensionMethodsFor(type))
            {
                DebugTrace.Trace(GetType().Name, "Added extension method matcher: " + methodInfo.Name);

                matchers.Add(new MethodNameMatcher(methodInfo));
            }

            return new ContextType(type, matchers);
        }

        private IEnumerable<IMemberMatcher> GetMatchers(MemberInfo member)
        {
            var reflectionMatcher = GetMemberMatcher(member);

            if (reflectionMatcher != null)
            {
                DebugTrace.Trace(GetType().Name, "Added reflection matcher: " + member.Name);

                return GetRegexMatchers(member).Union(new[] { reflectionMatcher});    
            }
            return GetRegexMatchers(member);
            
        }

        private IEnumerable<IMemberMatcher> GetRegexMatchers(MemberInfo member)
        {
            var regexAttrs = member.GetCustomAttributes(typeof(ContextRegexAttribute), true);
            foreach (var regexAttr in regexAttrs.Cast<ContextRegexAttribute>())
            {
                DebugTrace.Trace(GetType().Name, "Added regex matcher: " + member.Name + ", \"" + regexAttr.Pattern + "\"");

               yield return new RegexMatcher(regexAttr.Pattern, member);
            }
        }

        private static IMemberMatcher GetMemberMatcher(MemberInfo member)
        {
            if (member is MethodInfo)
                return new MethodNameMatcher((MethodInfo)member);

            return new PropertyOrFieldNameMatcher(member);
        }

        private MemberInfo[] GetMembers(Type type, BindingFlags flags)
        {
            var members = type.GetMembers(flags);

            return FilterMembers(members);
        }

        private MemberInfo[] FilterMembers(MemberInfo[] members)
        {
            var ignore = new[] { "GetType", "ToString", "CompareTo", "GetTypeCode", "Equals", "GetHashCode" };
            return members.Where(m => !(m.MemberType == MemberTypes.Constructor || m.MemberType == MemberTypes.NestedType || ignore.Contains(m.Name))).ToArray();
        }
    }
    public class ContextType
    {

        public IEnumerable<IMemberMatcher> MemberMatchers { get; private set; }

        public Type WrappedType { get; private set; }

        public ContextType(Type type, IEnumerable<IMemberMatcher> memberMatchers)
        {
            WrappedType = type;
            MemberMatchers = memberMatchers;
        }

       

        
    }
}