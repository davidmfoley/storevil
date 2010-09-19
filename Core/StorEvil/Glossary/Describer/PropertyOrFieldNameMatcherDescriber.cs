using StorEvil.Context.Matchers;
using StorEvil.Glossary;

namespace StorEvil.Core
{
    public class PropertyOrFieldNameMatcherDescriber : IStepDescriber
    {
        private readonly WordFilterDescriber _wordFilterDescriber = new WordFilterDescriber();
       
        public StepDescription Describe(StepDefinition stepDefinition)
        {
            var matcher = (PropertyOrFieldNameMatcher)stepDefinition.Matcher;
            return  _wordFilterDescriber.Describe(matcher.WordFilters);
        }
    }
}