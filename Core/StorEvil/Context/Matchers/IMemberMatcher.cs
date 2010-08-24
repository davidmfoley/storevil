using System;
using System.Collections.Generic;
using System.Reflection;
using StorEvil.Context.Matches;

namespace StorEvil.Context.Matchers
{
    public interface IMemberMatcher
    {
        MemberInfo MemberInfo { get; }
        IEnumerable<NameMatch> GetMatches(string line);
        Type ReturnType { get; }
    }
}