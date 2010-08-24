using System;
using System.Collections.Generic;
using StorEvil.Context.Matchers;

namespace StorEvil.Interpreter
{
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