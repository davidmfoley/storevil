using System;
using System.Collections.Generic;
using System.Reflection;

namespace StorEvil.Context
{
    public interface IMemberMatcher
    {
        NameMatch GetMatch(string line);
        MemberInfo MemberInfo { get; }
        IEnumerable<NameMatch> GetMatches(string line);
    }

    public class ContextRegexAttribute : Attribute
    {
        public ContextRegexAttribute(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get; set; }
    }
}