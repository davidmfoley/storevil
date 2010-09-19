using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;
using StorEvil.Context.WordFilters;
using StorEvil.Glossary;
using StorEvil.Interpreter;

namespace StorEvil.Core
{
    public interface IStepDescriber
    {
        StepDescription Describe(StepDefinition stepDefinition);
    }

    public class StepDescriber : IStepDescriber
    {
        public StepDescriber()
        {
            _describers.Add(typeof(MethodNameMatcher), new MethodNameMatcherDescriber());
            _describers.Add(typeof(RegexMatcher), new RegexMatcherDescriber());
            _describers.Add(typeof(PropertyOrFieldNameMatcher), new PropertyOrFieldNameMatcherDescriber());
        }

        Dictionary<Type, IStepDescriber> _describers = new Dictionary<Type, IStepDescriber>();

        public StepDescription Describe(StepDefinition stepDefinition)
        {
            var matcherType = stepDefinition.Matcher.GetType();
            if (_describers.ContainsKey(matcherType))
            {
                return _describers[matcherType].Describe(stepDefinition);                
            }

            return new StepDescription();
        }
    }

    public class PropertyOrFieldNameMatcherDescriber : IStepDescriber
    {
        private readonly WordFilterDescriber _wordFilterDescriber = new WordFilterDescriber();
       
        public StepDescription Describe(StepDefinition stepDefinition)
        {
            var matcher = (PropertyOrFieldNameMatcher)stepDefinition.Matcher;
            return  _wordFilterDescriber.Describe(matcher.WordFilters);
        }
    }

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

    public class MethodNameMatcherDescriber : IStepDescriber
    {
        private readonly WordFilterDescriber _wordFilterDescriber = new WordFilterDescriber();
        public StepDescription Describe(StepDefinition stepDefinition)
        {
            var methodNameMatcher = (MethodNameMatcher) stepDefinition.Matcher;

            return _wordFilterDescriber.Describe(methodNameMatcher.WordFilters);

        }
    }

    internal class WordFilterDescriber
    {     
        public StepDescription Describe(IEnumerable<WordFilter> filters)
        {
            if (filters.Count() > 1)
            {
                var first = TranslateWordFilter(filters.First(), false, true);
                var last = filters.Last();
                var middle = filters.Skip(1).Take(filters.Count() -2).Select(TranslateWordFilter);
            }
            return new StepDescription { Spans = spans };
        }

        private StepSpan TranslateWordFilter(WordFilter wordFilter, bool leadingSpace, bool trailingSpace)
        {
            if (wordFilter is TextMatchWordFilter)
                return new TextSpan(((TextMatchWordFilter) wordFilter).Word + " ");

            if (wordFilter is ParameterMatchWordFilter)
            {
                var paramMatch = ((ParameterMatchWordFilter)wordFilter);
                return new ParameterSpan(paramMatch.ParameterType, paramMatch.ParameterName);
            }

            return new TextSpan("");
        }
    }

    internal class ParameterDescriber
    {
        public string DescribeParameter( Type parameterType, string parameterName)
        {
            return string.Format("<{0} {1}>", TranslateTypeName(parameterType), parameterName);
        }

        private string TranslateTypeName(Type t)
        {
            if (t == typeof(int))
            {
                return "int";
            }
            if (t == typeof(double))
            {
                return "double";
            }
            if (t == typeof(string))
            {
                return "string";
            }
            if (t == typeof(bool))
            {
                return "bool";
            }
            return t.Name;
        }
    }
}