using StorEvil.Context.Matchers;
using StorEvil.Glossary;

namespace StorEvil.Core
{
    public class MethodNameMatcherDescriber : IStepDescriber
    {
        private readonly WordFilterDescriber _wordFilterDescriber = new WordFilterDescriber();
        public StepDescription Describe(StepDefinition stepDefinition)
        {
            var methodNameMatcher = (MethodNameMatcher) stepDefinition.Matcher;

            return _wordFilterDescriber.Describe(methodNameMatcher.WordFilters);

        }
    }
}