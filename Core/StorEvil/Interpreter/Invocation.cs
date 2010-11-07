using System;
using System.Collections.Generic;
using System.Reflection;

namespace StorEvil.Interpreter
{
    public class Invocation
    {
        public Invocation(MemberInfo info, IEnumerable<object> paramValues, IEnumerable<string> rawParamValues, string matchedText)
        {
            MemberInfo = info;
            ParamValues = paramValues;
            RawParamValues = rawParamValues;
            MatchedText = matchedText;
        }

        public IEnumerable<string> RawParamValues { get; set; }

        public string MatchedText
        {
            get; set;
        }

        public MemberInfo MemberInfo { get; set; }

        public IEnumerable<object> ParamValues { get; set; }
    }
}