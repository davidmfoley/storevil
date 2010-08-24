using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StorEvil.Interpreter
{
    internal class MemberReader
    {
        public IEnumerable<MemberInfo> GetMembers(Type type, BindingFlags flags)
        {
            var members = type.GetMembers(flags);

            return FilterMembers(members);
        }

        private MemberInfo[] FilterMembers(MemberInfo[] members)
        {
            var ignore = new[] {"GetType", "ToString", "CompareTo", "GetTypeCode", "Equals", "GetHashCode"};
            return
                members.Where(
                    m =>
                    !(m.MemberType == MemberTypes.Constructor || m.MemberType == MemberTypes.NestedType ||
                      ignore.Contains(m.Name))).ToArray();
        }
    }
}