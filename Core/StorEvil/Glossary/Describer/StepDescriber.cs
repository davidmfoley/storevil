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
            AddDescriber<MethodNameMatcher, MethodNameMatcherDescriber>();
            AddDescriber<RegexMatcher, RegexMatcherDescriber>();
            AddDescriber<PropertyOrFieldNameMatcher, PropertyOrFieldNameMatcherDescriber>();
        }

        readonly Dictionary<Type, IStepDescriber> _describers = new Dictionary<Type, IStepDescriber>();

        private  void AddDescriber<TMatcher, TDescriber>() where TDescriber : IStepDescriber, new()
        {
            _describers.Add(typeof(TMatcher), new TDescriber());
        }

        public StepDescription Describe(StepDefinition stepDefinition)
        {
            var matcherType = stepDefinition.Matcher.GetType();
            
            return _describers.ContainsKey(matcherType) ? 
                _describers[matcherType].Describe(stepDefinition) : 
                new StepDescription();
        }
    }
}