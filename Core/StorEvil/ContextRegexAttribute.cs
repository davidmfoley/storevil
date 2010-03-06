using System;

namespace StorEvil
{
    public class ContextRegexAttribute : Attribute
    {
        public ContextRegexAttribute(string pattern)
        {
            Pattern = pattern;
        }

        public string Pattern { get; set; }
    }
}