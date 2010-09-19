using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;
using StorEvil.Glossary;
using StorEvil.Interpreter;

namespace StorEvil.Core
{
    public class RegexMatcherDescriber : IStepDescriber
    {
        private readonly ParameterDescriber _parameterDescriber = new ParameterDescriber();
        public StepDescription Describe(StepDefinition stepDefinition)
        {
            var matcher = (RegexMatcher) stepDefinition.Matcher;
            try
            {                
                var pieces = Split(matcher.Pattern);

                List<StepSpan> spans = new List<StepSpan>();
            
                int currentParam = 0;
                foreach (var piece in pieces)
                {
                    if (piece.StartsWith("("))
                    {
                        var param = ((MethodInfo)matcher.MemberInfo).GetParameters().ElementAt(currentParam);
                        spans.Add(new ParameterSpan(param.ParameterType, param.Name));
                        currentParam++;
                    }
                    else
                    {
                        spans.Add(new TextSpan(piece));
                    }
                }

                return new StepDescription {Spans = spans};
            }
            catch(Exception ex)
            {
                DebugTrace.Trace(this, "Error occurred describing Regex: " + ex);
                return new StepDescription {Spans = new StepSpan[] {new TextSpan(matcher.Pattern)}};
            }
        }

       
        private string DescribeParameter(RegexMatcher matcher, int currentParam)
        {
            string s;
            var param = ((MethodInfo) matcher.MemberInfo).GetParameters().ElementAt(currentParam);
                   
            s = _parameterDescriber.DescribeParameter(param.ParameterType, param.Name);
            return s;
        }

        private IEnumerable<string> Split(string pattern)
        {

            pattern = ReplaceEscapes(pattern);
            var pos = 0;
            var prev = 0;

            while (pos < pattern.Length)
            {
                if (pattern[pos] == '(')
                {
                    yield return pattern.Substring(prev, pos - prev);
                    prev = pos;
                    while (pattern[pos] != ')' && pos < pattern.Length)
                    {
                        pos++;                        
                    }
                    yield return pattern.Substring(prev, pos - prev);
                    prev = pos + 1;
                }
                pos++;
            }
            if (prev < pattern.Length)
                yield return pattern.Substring(prev, pos - prev);
        }

        private string ReplaceEscapes(string pattern)
        {
            return pattern.Replace("\\\\", "\\").Replace("\\", "");
        }
    }
}