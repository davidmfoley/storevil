using System;
using System.Reflection;

namespace StorEvil.Context
{
    public interface IMemberMatcher
    {
        NameMatch GetMatch(string line);
        MemberInfo MemberInfo { get; }
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