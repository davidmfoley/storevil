using System.Collections.Generic;
using System.Reflection;
using StorEvil.Context.Matches;

namespace StorEvil.Context.Matchers
{
    public interface IMemberMatcher
    {
        NameMatch GetMatch(string line);
        MemberInfo MemberInfo { get; }
        IEnumerable<NameMatch> GetMatches(string line);
    }
}