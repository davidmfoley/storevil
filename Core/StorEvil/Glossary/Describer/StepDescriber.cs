using System;
using System.Collections.Generic;
using StorEvil.Context.Matchers;
using StorEvil.Glossary;

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
                return _describers[matcherType].Describe(stepDefinition);

            return new StepDescription();
        }
    }
}