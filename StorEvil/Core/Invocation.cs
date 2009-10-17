using System.Collections.Generic;
using System.Reflection;

namespace StorEvil.Core
{
    public class Invocation
    {
        public Invocation(MemberInfo info, IEnumerable<object> paramValues, string matchedText)
        {
            MemberInfo = info;
            ParamValues = paramValues;
            MatchedText = matchedText;
        }
        public string MatchedText
        {
            get; set;
        }

        public MemberInfo MemberInfo { get; set; }

        public IEnumerable<object> ParamValues { get; set; }
    }
}